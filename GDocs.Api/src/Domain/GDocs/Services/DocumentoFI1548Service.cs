using ICE.GDocs.Common.Core.Exceptions;
using ICE.GDocs.Domain.Core.Services;
using ICE.GDocs.Domain.Core.Transactions;
using ICE.GDocs.Domain.Core.Uow;
using ICE.GDocs.Domain.ExternalServices;
using ICE.GDocs.Domain.ExternalServices.Model;
using ICE.GDocs.Domain.Repositories;
using ICE.GDocs.Domain.Repositories.ProcessoAssinaturaDocumento;
using ICE.GDocs.Domain.Validation;
using ICE.GDocs.Domain.Validation.DocumentoFI1548;
using ICE.GDocs.Infra.CrossCutting.Models;
using ICE.GDocs.Infra.CrossCutting.Models.DocumentoFI1548.ViewModel;
using ICE.GDocs.Infra.CrossCutting.Models.Enums;
using ICE.GDocs.Infra.CrossCutting.Models.SaidaMaterialNotaFiscal;
using Microsoft.Extensions.Configuration;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using System.Xml.Linq;

namespace ICE.GDocs.Domain.Services
{
    internal class DocumentoFI1548Service : DomainService, IDocumentoFI1548Service
    {
        private readonly IDocumentoFI1548AgregationService _documentoFI1548AgregationService;

        private readonly IProcessoAssinaturaDocumentoService _processoAssinaturaDocumentoService;
        private readonly IUsuarioService _usuarioService;
        private readonly IGdocsWorkerExternalService _gdocsWorkerExternalService;
        private readonly IArquivoRepository _arquivoRepository;
        private readonly IInformacaoRepository _informacaoRepository;
        private readonly IConfiguration _configuration;
        private TryException<IEnumerable<AssinaturaInformacoesModel>> assinaturaInformacao = new TryException<IEnumerable<AssinaturaInformacoesModel>>();
        private const string TAG_DIR_MESMO_PASSO = "DIR-MESMO-PASSO";
        private const int TAG_BUSCA = 654263;

        public DocumentoFI1548Service(
            IDocumentoFI1548AgregationService documentoFI1548AgregationService,
            IProcessoAssinaturaDocumentoService processoAssinaturaDocumentoService,
            IUsuarioService usuarioService,
            IGdocsWorkerExternalService gdocsWorkerExternalService,
            IConfiguration configuration,
            IArquivoRepository arquivoRepository,
            IInformacaoRepository informacaoRepository,
            IUnitOfWork unitOfWork) : base(unitOfWork)
        {
            _documentoFI1548AgregationService = documentoFI1548AgregationService;
            _processoAssinaturaDocumentoService = processoAssinaturaDocumentoService;
            _gdocsWorkerExternalService = gdocsWorkerExternalService;
            _configuration = configuration;
            _arquivoRepository = arquivoRepository;
            _usuarioService = usuarioService;
            _informacaoRepository = informacaoRepository;
        }

        public async Task<TryException<DocumentoFI1548Model>> Adicionar(DocumentoFI1548Model documento, CancellationToken cancellationToken)
        {
            bool PodeEditar = documento.PodeEditar();
            using (var transaction = _unitOfWork.BeginTransaction())
            {
                var binarioSalvo = await _documentoFI1548AgregationService.BinarioRepository.Inserir(documento.ArquivoBinario, cancellationToken);

                documento.BinarioId = binarioSalvo.Success;

                var adicionarValidation = await new AdicionarValidation(documento).ValidateAsync(documento);
                if (!adicionarValidation.IsValid)
                    return adicionarValidation.Errors.ToValidationExceptionCollection();

                await _documentoFI1548AgregationService.LogService.AdicionarRastreabilidade(documento, "Documento validado.");

                var salvarDocumento = await _documentoFI1548AgregationService.DocumentoFI1548Repository.Salvar(documento, cancellationToken);

                if(PodeEditar){
                   await AddBinarioUpdatePasso(documento, cancellationToken);
                }

                _unitOfWork.Commit();

                return salvarDocumento;
            }
        }

        private async Task<TryException<Return>> AddBinarioUpdatePasso(DocumentoFI1548Model documento , CancellationToken cancellationToken)
        {
            var processoAssinaturaDocumentoOrigemNome = _configuration.GetValue("DocumentoFI1548:ProcessoAssinaturaDocumentoOrigemNome", string.Empty);

            var processoAssinaturaDocumentoOrigem = await _documentoFI1548AgregationService.ProcessoAssinaturaDocumentoOrigemRepository.ListarPorProcessoAssinaturaDocumentoOrigemIdENome(documento.Id.ToString(), processoAssinaturaDocumentoOrigemNome, cancellationToken);
            if (processoAssinaturaDocumentoOrigem.IsFailure)
                return processoAssinaturaDocumentoOrigem.Failure;

            if (processoAssinaturaDocumentoOrigem.Success != null)
            {
                var AssinaturaInformacao = await _informacaoRepository.Listar(new AssinaturaInformacoesFilterModel
                {
                    ListaDeprocessoAssinaturaDocumentoId = new List<int> {processoAssinaturaDocumentoOrigem.Success.ProcessoAssinaturaDocumentoId }
                }, cancellationToken);

                if (AssinaturaInformacao.IsFailure)
                    return AssinaturaInformacao.Failure;

                if(AssinaturaInformacao.Success.First().Destaque != documento.Destaque)
               {
                    AssinaturaInformacao.Success.First().Destaque = documento.Destaque;
                    await _informacaoRepository.Salvar(AssinaturaInformacao.Success.First(),cancellationToken);
                }

                await _documentoFI1548AgregationService.LogService.AdicionarRastreabilidade(documento, $"FI {documento.Id} Adicionando padu com novo binario: {processoAssinaturaDocumentoOrigem.Success.ProcessoAssinaturaDocumentoId}");
            

                var assinatura = new AssinaturaModel()
                {
                    Arquivos = new List<AssinaturaArquivoModel> { new AssinaturaArquivoModel() {
                        BinarioId = (long)documento.BinarioId,
                        Ordem = 0
                    }}
                };

                var arquivo = assinatura.Arquivos.First();
                assinatura.Arquivos = new List<AssinaturaArquivoModel>() { arquivo };

                var arquivoSalvo = await _arquivoRepository.Salvar(processoAssinaturaDocumentoOrigem.Success.ProcessoAssinaturaDocumentoId, assinatura.Arquivos, cancellationToken,documento.PodeEditar());
                if (arquivoSalvo.IsFailure)
                    return arquivoSalvo.Failure;

            }

            return Return.Empty;
        }

        public async Task<TryException<DocumentoFI1548Model>> Cancelar(DocumentoFI1548Model documento, UsuarioModel usuarioModel, bool permiteCancelarTodos, CancellationToken cancellationToken, byte[] novoArquivo = default)
        {
            if (documento.Status == DocumentoFI1548Status.Cancelado)
                return documento;

            using (var transaction = _unitOfWork.BeginTransaction())
            {
                var cancelarValidation = await new CancelarValidation(usuarioModel.ActiveDirectoryId, permiteCancelarTodos).ValidateAsync(documento);
                if (!cancelarValidation.IsValid)
                    return cancelarValidation.Errors.ToValidationExceptionCollection();

                documento.DefinirComoCancelado();

                if (novoArquivo != default) {
                    var binarioSalvo = await _documentoFI1548AgregationService.BinarioRepository.Inserir(novoArquivo, cancellationToken);
                    if (binarioSalvo.IsFailure)
                        return binarioSalvo.Failure;

                    documento.DefinirBinario(binarioSalvo.Success);
                }

                await _documentoFI1548AgregationService.LogService.AdicionarRastreabilidade(documento, "Documento cancelado.");

                var documentoSalvo = await _documentoFI1548AgregationService.DocumentoFI1548Repository.Salvar(documento, cancellationToken);

                if (documentoSalvo.IsFailure)
                    return documentoSalvo.Failure;

                var processoAssinaturaDocumentoOrigemNome = _configuration.GetValue<string>("DocumentoFI1548:ProcessoAssinaturaDocumentoOrigemNome", string.Empty);
                var processoList = await _documentoFI1548AgregationService.ProcessoAssinaturaDocumentoOrigemRepository.ListarProcessosAssinaturaPorOrigem(documento.Id.ToString(), processoAssinaturaDocumentoOrigemNome, cancellationToken);

                if (processoList.IsFailure)
                    return processoList.Failure;

                var alteracaoStatus = await AtualizarStatusEEmitirEventoAlteracaoParaCancelamento(processoList, cancellationToken);
                if (alteracaoStatus.IsFailure)
                    return alteracaoStatus.Failure;

                transaction.Commit();

                return documentoSalvo;
            }
        }

        public async Task<TryException<DocumentoFI1548Model>> Liquidar(DocumentoFI1548Model documento, bool permiteLiquidar, byte[] arquivo, CancellationToken cancellationToken)
        {
            if (documento.Status == DocumentoFI1548Status.Liquidado)
                return documento;

            using (var transaction = _unitOfWork.BeginTransaction())
            {
                var liquidarValidation = await new LiquidarValidation(permiteLiquidar).ValidateAsync(documento);

                if (!liquidarValidation.IsValid)
                    return liquidarValidation.Errors.ToValidationExceptionCollection();

                documento.DefinirComoLiquidado();

                await _documentoFI1548AgregationService.LogService.AdicionarRastreabilidade(documento, "Documento liquidado.");

                var binarioSalvo = await _documentoFI1548AgregationService.BinarioRepository.Inserir(arquivo, cancellationToken);
                if (binarioSalvo.IsFailure)
                    return binarioSalvo.Failure;

                documento.DefinirBinario(binarioSalvo.Success);

                var documentoSalvo = await _documentoFI1548AgregationService.DocumentoFI1548Repository.Salvar(documento, cancellationToken);

                _unitOfWork.Commit();

                return documentoSalvo;
            }
        }

        public async Task<TryException<int>> EnviarParaAssinatura(DocumentoEhAssinaturaPassosModel documentoEhAssinaturaPassosModel, string processoAssinaturaDocumentoOrigemNome, string titulo, string nomeDocumento, int categoriaId, CancellationToken cancellationToken)
        {
            var configuracaoCategoria = await _documentoFI1548AgregationService.ConfiguracaoRepository.ObterConfiguracao(_configuration.GetValue("ChaveConfiguracao:Categoria", "configCategorias"), cancellationToken);
            if (configuracaoCategoria.IsFailure)
                return configuracaoCategoria.Failure;

            var configCategorias = Newtonsoft.Json.JsonConvert.DeserializeObject<System.Collections.Generic.List<ConfiguracaoCategoriaModel>>(configuracaoCategoria.Success.Valor);

            var passosItensTemplateDirRetorno = await _documentoFI1548AgregationService.TemplateProcessoAssinaturaDocumentoRepository.ListarPassosEUsuariosPorTag(TAG_DIR_MESMO_PASSO, cancellationToken);
            if (passosItensTemplateDirRetorno.IsFailure)
                return passosItensTemplateDirRetorno.Failure;

            using (var transaction = _unitOfWork.BeginTransaction())
            {
                var assinatura = new AssinaturaModel()
                {
                    Arquivos = new List<AssinaturaArquivoModel> { new AssinaturaArquivoModel() {
                        BinarioId = documentoEhAssinaturaPassosModel.DocumentoModel.BinarioId.Value,
                        Ordem = 1
                    }},

                    Informacoes = new AssinaturaInformacoesModel()
                    {
                        Titulo = titulo,
                        NomeDocumento = nomeDocumento,
                        Descricao = documentoEhAssinaturaPassosModel.DocumentoModel.Descricao,
                        UsuarioGuidAd = documentoEhAssinaturaPassosModel.DocumentoModel.AutorId,
                        Status = StatusAssinaturaDocumento.EmConstrucao,
                        NumeroDocumento = documentoEhAssinaturaPassosModel.DocumentoModel.Numero,
                        CategoriaId = categoriaId, 
                        Destaque = documentoEhAssinaturaPassosModel.DocumentoModel.Destaque
                    },

                    Passos = new AssinaturaPassoModel()
                    {
                        AdicionarDir = true,
                        NotificarFinalizacaoDir = false
                    }
                };

                assinatura.Passos.DefinirItens(documentoEhAssinaturaPassosModel.AssinaturaPassoModel.Itens);
                assinatura.Passos.DefinirItensDir(documentoEhAssinaturaPassosModel.AssinaturaPassoModel.ItensDir);

                var processoAssinaturaDocumentoService = await _processoAssinaturaDocumentoService.Salvar(assinatura, "", configCategorias, cancellationToken);
                if (processoAssinaturaDocumentoService.IsFailure)
                    return processoAssinaturaDocumentoService.Failure;

                await _documentoFI1548AgregationService.ProcessoAssinaturaDocumentoOrigemRepository.Inserir(new ProcessoAssinaturaDocumentoOrigemModel()
                {
                    ProcessoAssinaturaDocumentoId = processoAssinaturaDocumentoService.Success.Informacoes.Id,
                    ProcessoAssinaturaDocumentoOrigemId = documentoEhAssinaturaPassosModel.DocumentoModel.Id.ToString(),
                    ProcessoAssinaturaDocumentoOrigemNome = processoAssinaturaDocumentoOrigemNome
                }, cancellationToken);

                transaction.Commit();

                return processoAssinaturaDocumentoService.Success.Informacoes.Id;
            }
        }

        private async Task<TryException<Return>> AtualizarStatusEEmitirEventoAlteracaoParaCancelamento(TryException<IEnumerable<ProcessoAssinaturaDocumentoOrigemModel>> processoList, CancellationToken cancellationToken)
        {
            var processoListId = processoList.Success?.Select(pado => pado.ProcessoAssinaturaDocumentoId);

            if (processoListId?.Any() != null)
            {
                var atualizarProcessosList = await _processoAssinaturaDocumentoService.AtualizarStatusProcessos(processoListId, StatusAssinaturaDocumento.Cancelado, cancellationToken);
                if (atualizarProcessosList.IsFailure)
                    return atualizarProcessosList.Failure;

                foreach (var pad in atualizarProcessosList.Success)
                {
                    var emitirAlteracaoStatusDocumento = await _gdocsWorkerExternalService.EmitirAlteracaoStatusDocumento(pad.Id);
                    if (emitirAlteracaoStatusDocumento.IsFailure)
                        return emitirAlteracaoStatusDocumento.Failure;
                }
            }

            return Return.Empty;
        }

        public async Task<TryException<IEnumerable<AssinaturaArquivoModel>>> ListarArquivosUploadPorPadId(int processoAssinaturaDocumentoId, CancellationToken cancellationToken, bool listarTodos = false) =>
         await _arquivoRepository.ListarArquivosUploadPorPadId(processoAssinaturaDocumentoId,cancellationToken, listarTodos);

        public async Task<TryException<Return>> InativarFluxoAssinatura(int processoAssinaturaDocumentoId, CancellationToken cancellationToken)
        {
            using (var transaction = _unitOfWork.BeginTransaction())
            {
                await _documentoFI1548AgregationService.LogService.AdicionarRastreabilidade(processoAssinaturaDocumentoId, "Iniciando inativaçao do fluxo.");

                var inativarDocOrigem = await _documentoFI1548AgregationService.ProcessoAssinaturaDocumentoOrigemRepository.InativarAssDocumentoOrigem(processoAssinaturaDocumentoId, cancellationToken);
                if (inativarDocOrigem.IsFailure)
                    return inativarDocOrigem.Failure;


                var InativarUsuariosPasso = await _processoAssinaturaDocumentoService.InativarUsuariosDosPassos(processoAssinaturaDocumentoId, cancellationToken);
                if (InativarUsuariosPasso.IsFailure)
                    return InativarUsuariosPasso.Failure;


                var inativarPassos = await _processoAssinaturaDocumentoService.InativarPassos(processoAssinaturaDocumentoId, cancellationToken);
                if (inativarPassos.IsFailure)
                    return inativarPassos.Failure;


                var inativarInformacao = await _informacaoRepository.InativarInformacao(processoAssinaturaDocumentoId, cancellationToken);
                if (inativarInformacao.IsFailure)
                    return inativarInformacao.Failure;

                 await _documentoFI1548AgregationService.LogService.AdicionarRastreabilidade(processoAssinaturaDocumentoId, "Finalizando Inativaçao do Fluxo.");

                _unitOfWork.Commit();
            }


            return Return.Empty;
        }

        public async Task<TryException<ProcessoAssinaturaDocumentoOrigemModel>> ListarPorProcessoAssinaturaDocumentoOrigemIdENome(string ProcessoAssinaturaDocumentoOrigemId, string ProcessoAssinaturaDocumentoOrigemNome, CancellationToken cancellationToken)
            => await _documentoFI1548AgregationService.ProcessoAssinaturaDocumentoOrigemRepository.ListarPorProcessoAssinaturaDocumentoOrigemIdENome(ProcessoAssinaturaDocumentoOrigemId, ProcessoAssinaturaDocumentoOrigemNome, cancellationToken);

        public async Task<TryException<IEnumerable<AssinaturaPassoItemModel>>> ObterPassosEhUsuariosPorId(int processoAssinaturaDocumentoId, string origemDocumento,CancellationToken cancellationToken)
        {
            var origemDocumentoFi = await _documentoFI1548AgregationService.ProcessoAssinaturaDocumentoOrigemRepository.ListarPorProcessoAssinaturaDocumentoOrigemIdENome(processoAssinaturaDocumentoId.ToString(), origemDocumento, cancellationToken);
            if (origemDocumentoFi.IsFailure)
                return origemDocumentoFi.Failure;

            if (origemDocumentoFi.Success == null){
                return new List<AssinaturaPassoItemModel>();
            }


            var assinaturaPasso = await _processoAssinaturaDocumentoService.ObterPassosEhUsuariosPorId(origemDocumentoFi.Success.ProcessoAssinaturaDocumentoId, cancellationToken);
            if (assinaturaPasso.IsFailure)
                return assinaturaPasso.Failure;


            return assinaturaPasso;
        }

        public async Task<TryException<int>> InserirSolicitacaoCiencia(DocumentoFI1548CienciaModel documentoFI1548CienciaModel, CancellationToken cancellationToken)
        {
            using (var transaction = _unitOfWork.BeginTransaction())
            {
                var result = await _documentoFI1548AgregationService.DocumentoFI1548Repository.Inserir(documentoFI1548CienciaModel, cancellationToken);
                transaction.Commit();
                return result;
            }
        }

        public async Task<TryException<DocumentoFI1548Model>> AtualizarStatusDoDocumento(DocumentoFI1548Model documentoFI1548Model, CancellationToken cancellationToken)
        {
            using(var transaction = _unitOfWork.BeginTransaction())
            {
                var result = await _documentoFI1548AgregationService.DocumentoFI1548Repository.Salvar(documentoFI1548Model, cancellationToken);
                return result;
            }
        }

        public async Task<TryException<IEnumerable<ProcessoAssinaturaDocumentoModel>>> ListarCienciasPendentesDeAprovacaoPorUsuario(Guid activeDirectoryId, CancellationToken cancellationToken)
            => await _documentoFI1548AgregationService.DocumentoFI1548Repository.ListarCienciasPendentesDeAprovacaoPorUsuario(activeDirectoryId, cancellationToken);

        public async Task<TryException<DocumentoFI1548CienciaModel>> ObterCienciaPorId(int idSolicitacaoCiencia, CancellationToken cancellationToken)
        => await _documentoFI1548AgregationService.DocumentoFI1548Repository.ObterCienciaPorId(idSolicitacaoCiencia, cancellationToken);

        public async Task<TryException<IEnumerable<CienciaUsuarioAprovacaoModel>>> ListarObsUsuarioPorCiencia(int idSolicitacaoCiencia, CancellationToken cancellationToken)
        => await _documentoFI1548AgregationService.DocumentoFI1548Repository.ListarObsUsuarioPorCiencia(idSolicitacaoCiencia, cancellationToken);

        public async Task<TryException<SoclicitacaoCienciaAprovadoresModel>> ObterAprovadoresCienciaCancelamento(int documentoId, CancellationToken cancellationToken) => 
                   await _documentoFI1548AgregationService.DocumentoFI1548Repository.ObterAprovadoresCienciaCancelamento(documentoId, cancellationToken);

        public async Task<TryException<bool>> RegistroCienciaPorUsuario(DocumentoFI1548CienciaModel documentoFI1548CienciaModel, UsuarioModel usuarioModel, CancellationToken cancellationToken)
        {
            TryException<bool> result = false;

            using (var transaction = _unitOfWork.BeginTransaction())
            {
                var idCienciaAprovador = await _documentoFI1548AgregationService.DocumentoFI1548Repository.ObterAprovadoresPorUsuarioECiencia(documentoFI1548CienciaModel.CienciaId, usuarioModel.ActiveDirectoryId, cancellationToken);

                if (idCienciaAprovador.IsFailure)
                    return idCienciaAprovador.Failure;

                if (idCienciaAprovador.Success == null)
                    return new BusinessException($"Não foi encontrado nenhuma aprovação pendente. Para solicitação de número: {documentoFI1548CienciaModel.DocumentoFI1548Numero}");

                await _documentoFI1548AgregationService.DocumentoFI1548Repository.RegistroCiencia(documentoFI1548CienciaModel, idCienciaAprovador.Success.Id, cancellationToken);

                var aprovadorCiencia = await _documentoFI1548AgregationService.DocumentoFI1548Repository.ObterAprovadoresPorCiencia(documentoFI1548CienciaModel.CienciaId, cancellationToken);

                if (aprovadorCiencia.IsFailure)
                    return aprovadorCiencia.Failure;

                result = await ValidarAprovacaoFinal(aprovadorCiencia.Success, documentoFI1548CienciaModel, usuarioModel, transaction, cancellationToken);

                transaction.Commit();
            }

            return result;
        }

        private async Task<TryException<bool>> ValidarAprovacaoFinal
        (
            IEnumerable<CienciaUsuariosProvacao> aprovadorCiencia, DocumentoFI1548CienciaModel documentoFI1548CienciaModel, 
            UsuarioModel usuarioModel, IDataTransaction transaction, CancellationToken cancellationToken
        )
        {
            if (aprovadorCiencia.All(x => x.Aprovacao != null))
            {
                if (aprovadorCiencia.Any(x => x.FlgRejeitado))
                    return await ProcessarRejeitado(documentoFI1548CienciaModel, usuarioModel, transaction, cancellationToken);

                return await ProcessarAceito(documentoFI1548CienciaModel, usuarioModel, cancellationToken);
            }

            return false;
        }

        private async Task<TryException<bool>> ProcessarRejeitado(DocumentoFI1548CienciaModel documentoFI1548CienciaModel, UsuarioModel usuarioModel, IDataTransaction transaction, CancellationToken cancellationToken)
        {
            documentoFI1548CienciaModel.IdStatusCiencia = (int)StatusCienciaFi1548.Rejeitado;
            await _documentoFI1548AgregationService.DocumentoFI1548Repository.AtualizarCienciaStatus(documentoFI1548CienciaModel, cancellationToken);

            transaction.Commit();

            var ciencias = await _documentoFI1548AgregationService.DocumentoFI1548Repository.ListarObsUsuarioPorCiencia(documentoFI1548CienciaModel.CienciaId, cancellationToken);

            if (ciencias.IsFailure)
                return ciencias.Failure;

            documentoFI1548CienciaModel.CienciaUsuarioAprovacao = ciencias.Success;

            await ProcessarCancelamento(false, documentoFI1548CienciaModel, usuarioModel, cancellationToken);

            return false;
        }

        private async Task<TryException<bool>> ProcessarAceito(DocumentoFI1548CienciaModel documentoFI1548CienciaModel, UsuarioModel usuarioModel, CancellationToken cancellationToken)
        {
            documentoFI1548CienciaModel.IdStatusCiencia = (int)StatusCienciaFi1548.Concluido;
            await _documentoFI1548AgregationService.DocumentoFI1548Repository.AtualizarCienciaStatus(documentoFI1548CienciaModel, cancellationToken);

            await ProcessarCancelamento(true, documentoFI1548CienciaModel, usuarioModel, cancellationToken);
            return true;
        }

        private async Task<TryException<Return>> ProcessarCancelamento(bool aceito, DocumentoFI1548CienciaModel documentoFI1548CienciaModel, UsuarioModel usuarioModel, CancellationToken cancellationToken)
        {
            DocumentoFI1548Status documentoFI1548Status;

            var email = new EmailModel
            {
                Responsavel = documentoFI1548CienciaModel.NomeUsuario,
                Nome = usuarioModel.Nome,
                Numero = documentoFI1548CienciaModel.DocumentoFI1548Numero.ToString(),
                DataEnvio = DateTime.Now.ToString("dd/MM/yyyy"),
                Destinatario = usuarioModel.Email
            };

            if (aceito)
            {
                email.Template = _configuration.GetSection("TemplatesEmail:StatusPagamento-CancelamentoAceito").Value;
                email.Assunto = "GDocs - Cancelamento aceito";
                documentoFI1548Status = DocumentoFI1548Status.EmConstrucao ;
            }
            else
            {
                var cienciasFI1548 = await _documentoFI1548AgregationService.DocumentoFI1548Repository.ListarObsUsuarioPorCiencia(documentoFI1548CienciaModel.CienciaId, cancellationToken);

                if (cienciasFI1548.IsFailure)
                    return cienciasFI1548.Failure;

                var rejeicoesFI1548 = cienciasFI1548.Success.Where(a => a.FlgRejeitado).AsList();

                foreach(var rejeicao in rejeicoesFI1548)
                {
                    var dadosUsuarioAd = await _usuarioService.ObterUsuarioActiveDirectoryPorId(rejeicao.UsuarioGuid, cancellationToken);

                    email.Texto.Add(new RejeicaoDiretorModel
                    {
                        NomeDiretor = dadosUsuarioAd.Success.Nome,
                        Observacao = rejeicao.Observacao,
                        DataRejeicao = $"{rejeicao.DataAprovacao: dd/MM/yyyy}"
                    });
                }

                email.Template = _configuration.GetSection("TemplatesEmail:StatusPagamento-CancelamentoRejeitado").Value;
                email.Assunto = "GDocs - Cancelamento rejeitado";
                documentoFI1548Status = DocumentoFI1548Status.EmAberto;
            }

            await _documentoFI1548AgregationService.DocumentoFI1548Repository.AtualizarStatus(documentoFI1548CienciaModel.DocumentoFI1548Id, documentoFI1548Status, cancellationToken);
            await _documentoFI1548AgregationService.EmailExternalService.Enviar(email);

            return Return.Empty;
        }

        public async Task<TryException<IEnumerable<AssinaturaInformacoesModel>>> ObterReferenciaSubstitutoOrigemDocumento(long processoAssinaturaOrigem, CancellationToken cancellationToken)
        {

            var documentoSubstituidoId = await _documentoFI1548AgregationService.DocumentoFI1548Repository.ObterReferenciaSubstituto(processoAssinaturaOrigem, cancellationToken);
            
            if (documentoSubstituidoId.Success != 0)
            {
                assinaturaInformacao = await _informacaoRepository.Listar(new AssinaturaInformacoesFilterModel() { NumeroDocumento = documentoSubstituidoId.Success }, cancellationToken);
            }
            else
            {
                assinaturaInformacao = await _informacaoRepository.Listar(new AssinaturaInformacoesFilterModel() { NumeroDocumento = TAG_BUSCA }, cancellationToken);
            }

            return assinaturaInformacao;
        }
        public async Task<TryException<IEnumerable<ObterMotivoCancelamentoModel>>> ObterMotivoCancelamento(int documentoId, CancellationToken cancellationToken)
        {

            TryException<IEnumerable<ObterMotivoCancelamentoModel>> motivoCancelamento = new TryException<IEnumerable<ObterMotivoCancelamentoModel>>();

            var documento = await _documentoFI1548AgregationService.DocumentoFI1548Repository.ObterStatusPagamentoPorId(documentoId, cancellationToken);

            if (documento.IsFailure)
                return new TryException<IEnumerable<ObterMotivoCancelamentoModel>>();

            if(documento.Success.Status == DocumentoFI1548Status.Cancelado)
            {
               motivoCancelamento = await _documentoFI1548AgregationService.DocumentoFI1548Repository.ObterMotivoCancelamento(documentoId, cancellationToken);
            } 
            else if(documento.Success.Status == DocumentoFI1548Status.Reprovado)
            {
                motivoCancelamento = await _documentoFI1548AgregationService.DocumentoFI1548Repository.ObterJustificativaReprovacao(documentoId, cancellationToken);
            }

            if (motivoCancelamento.IsFailure)
                return motivoCancelamento;

            foreach(var motivo in motivoCancelamento.Success)
            {
               var usuario = await _usuarioService.ListarPerfisUsuarioPorGuid(motivo.UsuarioGuid, cancellationToken);

                if(usuario.Success != null)
                {
                    motivo.Nome = usuario.Success.Nome;
                }
               
            }

            return motivoCancelamento;
        }
                   



    }
}
