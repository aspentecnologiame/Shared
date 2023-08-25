using ICE.GDocs.Domain.Core.Services;
using ICE.GDocs.Domain.Core.Uow;
using ICE.GDocs.Domain.ExternalServices;
using ICE.GDocs.Domain.GDocs.Repositories.SolicitacaoSaidaMaterial;
using ICE.GDocs.Domain.GDocs.Validation.SolicitacaoSaidaMaterial;
using ICE.GDocs.Domain.Repositories;
using ICE.GDocs.Domain.Repositories.ProcessoAssinaturaDocumento;
using ICE.GDocs.Domain.Services;
using ICE.GDocs.Domain.Validation;
using ICE.GDocs.Domain.Validation.SolicitacaoSaidaMaterial;
using ICE.GDocs.Infra.CrossCutting.Models;
using ICE.GDocs.Infra.CrossCutting.Models.Enums;
using Microsoft.Extensions.Configuration;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using System.Xml.Linq;

namespace ICE.GDocs.Domain.GDocs.Services.SolicitacaoSaidaMaterial
{
    internal class SolicitacaoSaidaMaterialService : DomainService, ISolicitacaoSaidaMaterialService
    {
        private readonly ISolicitacaoSaidaMaterialRepository _solicitacaoSaidaMaterialRepository;
        private readonly ISolicitacaoSaidaMaterialAgregationService _solicitacaoSaidaMaterialAgregationService;
        private readonly IRdlToPdfBytesConverterExternalService _rdlToPdfBytesConverterExternalService;
        private readonly IConfiguration _configuration;
        private readonly IUsuarioService _usuarioService;
        private readonly IBinarioRepository _binarioRepository;
        private readonly IArquivoRepository _arquivoRepository;

        private const string TAG_DIR_MESMO_PASSO = "DIR-MESMO-PASSO";

        public SolicitacaoSaidaMaterialService(
            IUnitOfWork unitOfWork,
            ISolicitacaoSaidaMaterialRepository solicitacaoSaidaMaterialRepository,
            IBinarioRepository binarioRepository,
            IUsuarioService usuarioService,
            IConfiguration configuration,
            IRdlToPdfBytesConverterExternalService rdlToPdfBytesConverterExternalService,
            ISolicitacaoSaidaMaterialAgregationService solicitacaoSaidaMaterialAgregationService,
            IArquivoRepository arquivoRepository) : base(unitOfWork)
        {
            _solicitacaoSaidaMaterialRepository = solicitacaoSaidaMaterialRepository;
            _binarioRepository = binarioRepository;
            _usuarioService = usuarioService;
            _configuration = configuration;
            _rdlToPdfBytesConverterExternalService = rdlToPdfBytesConverterExternalService;
            _solicitacaoSaidaMaterialAgregationService = solicitacaoSaidaMaterialAgregationService;
            _arquivoRepository = arquivoRepository;
        }


        public async Task<TryException<SolicitacaoSaidaMaterialModel>> ObterPorId(int id, CancellationToken cancellationToken)
        => await _solicitacaoSaidaMaterialRepository.ObterPorId(id, cancellationToken);


        public async Task<TryException<SolicitacaoSaidaMaterialModel>> ObterMaterialComItensPorId(int id, CancellationToken cancellationToken)
        {
           return await _solicitacaoSaidaMaterialRepository.ObterMaterialComItensPorId(id, cancellationToken);
        }

        public async Task<TryException<SolicitacaoSaidaMaterialModel>> Atualizar(SolicitacaoSaidaMaterialModel solicitacaoSaidaMaterialModel, CancellationToken cancellationToken)
        => await _solicitacaoSaidaMaterialRepository.Atualizar(solicitacaoSaidaMaterialModel, cancellationToken);
        public async Task<TryException<SolicitacaoSaidaMaterialModel>> Inserir(SolicitacaoSaidaMaterialModel solicitacaoSaidaMaterialModel, CancellationToken cancellationToken)
        => await _solicitacaoSaidaMaterialRepository.Inserir(solicitacaoSaidaMaterialModel, cancellationToken);

        public async Task<TryException<Return>> Excluir(int id, CancellationToken cancellationToken)
        => await _solicitacaoSaidaMaterialRepository.Excluir(id);

        public async Task<TryException<SolicitacaoSaidaMaterialModel>> Adicionar(SolicitacaoSaidaMaterialModel solicitacaoSaidaMaterialModel, CancellationToken cancellationToken)
        {

            // verificando se já exite assinatura para documento
            var processoAssinaturaOrigemNome = _configuration.GetValue("DocumentoFI347:ProcessoAssinaturaDocumentoOrigemNome", string.Empty);

            var processoAssinaturaOrigem = await _solicitacaoSaidaMaterialAgregationService.ProcessoAssinaturaDocumentoOrigemRepository.ListarPorProcessoAssinaturaDocumentoOrigemIdENome(solicitacaoSaidaMaterialModel.Id.ToString(), processoAssinaturaOrigemNome, cancellationToken);
            if (processoAssinaturaOrigem.IsFailure)
                return processoAssinaturaOrigem.Failure;


            var cadastroValidadtion = await new CadastroValidation(solicitacaoSaidaMaterialModel).ValidateAsync(solicitacaoSaidaMaterialModel);
            if (!cadastroValidadtion.IsValid)
                return cadastroValidadtion.Errors.ToValidationExceptionCollection();

            var saidaMaterialId = await _solicitacaoSaidaMaterialRepository.Inserir(solicitacaoSaidaMaterialModel, cancellationToken);

            if (saidaMaterialId.IsFailure)
                return saidaMaterialId.Failure;

            solicitacaoSaidaMaterialModel.Id = saidaMaterialId.Success.Id;

           var AddEditarItem =  await AdicionaEditarItemMaterial(solicitacaoSaidaMaterialModel, cancellationToken);
            if (AddEditarItem.IsFailure)
                return AddEditarItem.Failure;


            var obterRelatorio = await ObterRelatorio(solicitacaoSaidaMaterialModel, cancellationToken);

            if (obterRelatorio.IsFailure)
                return obterRelatorio.Failure;

            var binarioSalvo = await _binarioRepository.Inserir(obterRelatorio.Success, cancellationToken);

            var binarioID = binarioSalvo.Success;

            solicitacaoSaidaMaterialModel.BinarioId = binarioID;

            // se for editar processo de criaçao de uma pad
            if (processoAssinaturaOrigem.Success != null)
            {
                await EditarBinario(solicitacaoSaidaMaterialModel, processoAssinaturaOrigem.Success.ProcessoAssinaturaDocumentoId, cancellationToken);
            }

            var atualizar = await _solicitacaoSaidaMaterialRepository.Atualizar(solicitacaoSaidaMaterialModel, cancellationToken);

            if (atualizar.IsFailure)
                return atualizar.Failure;

            return saidaMaterialId;
        }

        private async Task<TryException<Return>> AdicionaEditarItemMaterial(SolicitacaoSaidaMaterialModel solicitacaoSaidaMaterialModel, CancellationToken cancellationToken)
        {
            var ItemGravado  = await _solicitacaoSaidaMaterialAgregationService.SolicitacaoSaidaMaterialItemRepository.ObterPorIdSolicitacaoSaidaMaterial(solicitacaoSaidaMaterialModel.Id, cancellationToken);

            if (ItemGravado.IsFailure)
                return ItemGravado.Failure;

            var MateriaisPraDesativar = ItemGravado.Success
                                        .Select(x => x.IdSolicitacaoSaidaMaterialItem)
                                        .Where(y => y.NotIn(solicitacaoSaidaMaterialModel.ItemMaterial.Select(x => x.IdSolicitacaoSaidaMaterialItem)
                                        .ToArray()));

            if (MateriaisPraDesativar.Any())
                await _solicitacaoSaidaMaterialAgregationService.SolicitacaoSaidaMaterialItemRepository.DesativarItemMaterial(MateriaisPraDesativar, cancellationToken);

            foreach (var solicitacaoSaidaItemMaterial in solicitacaoSaidaMaterialModel.ItemMaterial)
            {
                solicitacaoSaidaItemMaterial.IdSolicitacaoSaidaMaterial = solicitacaoSaidaMaterialModel.Id;

                var cadastroValidadtion = await new CadastroItemValidator().ValidateAsync(solicitacaoSaidaItemMaterial);
                if (!cadastroValidadtion.IsValid)
                    return cadastroValidadtion.Errors.ToValidationExceptionCollection();

                await _solicitacaoSaidaMaterialAgregationService.SolicitacaoSaidaMaterialItemRepository.Inserir(solicitacaoSaidaItemMaterial, cancellationToken);
            }

            return Return.Empty;
        }

        public async Task<TryException<byte[]>> ObterRelatorio(SolicitacaoSaidaMaterialModel solicitacaoSaidaMaterialModel, CancellationToken cancellationToken)
        {
            var parametros = new Dictionary<string, string>();

            var obterUsuarioActiveDirectoryPorId = await _usuarioService.ObterUsuarioActiveDirectoryPorId(solicitacaoSaidaMaterialModel.GuidResponsavel, cancellationToken);

            if (obterUsuarioActiveDirectoryPorId.IsFailure)
                return obterUsuarioActiveDirectoryPorId.Failure;

            var nomeCompleto = obterUsuarioActiveDirectoryPorId.Success?.Nome?.Split(' ')?.ToList();
            var nomeResponsavelSaida = $"{nomeCompleto?[0]} {nomeCompleto?[nomeCompleto.Count - 1]}";

            parametros.Add("SSM_IDT", $"{solicitacaoSaidaMaterialModel.Id}");
            parametros.Add("nomeResponsavelSaida", $"{nomeResponsavelSaida ?? solicitacaoSaidaMaterialModel.GuidResponsavel.ToString()}");

            var responseBinario = _rdlToPdfBytesConverterExternalService.Converter(
                         _configuration.GetValue("DocumentoFI347:RdlPath", string.Empty),
                         parametros);

            if (responseBinario.IsFailure)
                return responseBinario.Failure;

            return responseBinario.Success;
        }

        public async Task<TryException<int>> EnviarParaAssinatura(SolicitacaoSaidaMaterialModel documento, string processoAssinaturaDocumentoOrigemNome, string titulo, string nomeDocumento, int categoriaId, CancellationToken cancellationToken)
        {
            var configuracaoCategoria = await _solicitacaoSaidaMaterialAgregationService.ConfiguracaoRepository.ObterConfiguracao(_configuration.GetValue("ChaveConfiguracao:Categoria", "configCategorias"), cancellationToken);
            if (configuracaoCategoria.IsFailure)
                return configuracaoCategoria.Failure;

            var configCategorias = Newtonsoft.Json.JsonConvert.DeserializeObject<List<ConfiguracaoCategoriaModel>>(configuracaoCategoria.Success.Valor);

            var passosItensTemplateDirRetorno = await _solicitacaoSaidaMaterialAgregationService.TemplateProcessoAssinaturaDocumentoRepository.ListarPassosEUsuariosPorTag(TAG_DIR_MESMO_PASSO, cancellationToken);
            if (passosItensTemplateDirRetorno.IsFailure)
                return passosItensTemplateDirRetorno.Failure;

            using (var transaction = _unitOfWork.BeginTransaction())
            {
                var assinatura = new AssinaturaModel()
                {
                    Arquivos = new List<AssinaturaArquivoModel> { new AssinaturaArquivoModel() {
                        BinarioId = documento.BinarioId,
                        Ordem = 1
                    }},

                    Informacoes = new AssinaturaInformacoesModel()
                    {
                        Titulo = titulo,
                        NomeDocumento = nomeDocumento,
                        Descricao = documento.Motivo,
                        UsuarioGuidAd = documento.GuidResponsavel,
                        Status = StatusAssinaturaDocumento.EmConstrucao,
                        NumeroDocumento = documento.Numero,
                        CategoriaId = categoriaId,
                        Destaque = false
                    },

                    Passos = new AssinaturaPassoModel()
                    {
                        AdicionarDir = true,
                        NotificarFinalizacaoDir = false
                    }
                };

                assinatura.Passos.DefinirItensDir(passosItensTemplateDirRetorno.Success.ToList());

                var processoAssinaturaDocumentoService = await _solicitacaoSaidaMaterialAgregationService.ProcessoAssinaturaDocumentoService.Salvar(assinatura, "", configCategorias, cancellationToken);
                if (processoAssinaturaDocumentoService.IsFailure)
                    return processoAssinaturaDocumentoService.Failure;

                await _solicitacaoSaidaMaterialAgregationService.ProcessoAssinaturaDocumentoOrigemRepository.Inserir(new ProcessoAssinaturaDocumentoOrigemModel()
                {
                    ProcessoAssinaturaDocumentoId = processoAssinaturaDocumentoService.Success.Informacoes.Id,
                    ProcessoAssinaturaDocumentoOrigemId = documento.Id.ToString(),
                    ProcessoAssinaturaDocumentoOrigemNome = processoAssinaturaDocumentoOrigemNome
                }, cancellationToken);

                transaction.Commit();

                return processoAssinaturaDocumentoService.Success.Informacoes.Id;
            }
        }

        public async Task<TryException<SolicitacaoSaidaMaterialModel>> Cancelar(SolicitacaoSaidaMaterialModel saidaMaterial, UsuarioModel usuarioModel, bool permiteCancelarTodos, CancellationToken cancellationToken, byte[] novoArquivo = default)
        {
            if (saidaMaterial.StatusId == SolicitacaoSaidaMaterialStatus.Cancelado.ToInt32())
                return saidaMaterial;

            using (var transaction = _unitOfWork.BeginTransaction())
            {
                var validation = await new CancelarValidation(usuarioModel.ActiveDirectoryId, permiteCancelarTodos).ValidateAsync(saidaMaterial);

                if (!validation.IsValid)
                    return validation.Errors.ToValidationExceptionCollection();

                saidaMaterial.DefinirComoCancelado();

                if (novoArquivo != default)
                {
                    var binarioSalvo = await _binarioRepository.Inserir(novoArquivo, cancellationToken);
                    if (binarioSalvo.IsFailure)
                        return binarioSalvo.Failure;

                    saidaMaterial.DefinirBinario(binarioSalvo.Success);
                }

                await _solicitacaoSaidaMaterialAgregationService.LogService.AdicionarRastreabilidade(saidaMaterial, "Solicitação de saída de material cancelada.");

                var cancelamentoSaidaMaterial = await _solicitacaoSaidaMaterialRepository.AtualizarStatus(saidaMaterial.Id, SolicitacaoSaidaMaterialStatus.Cancelado, cancellationToken);

                if (cancelamentoSaidaMaterial.IsFailure)
                    return cancelamentoSaidaMaterial.Failure;

                var processoAssinaturaDocumentoOrigemNome = _configuration.GetValue<string>("DocumentoFI347:ProcessoAssinaturaDocumentoOrigemNome", string.Empty);
                var processoList = await _solicitacaoSaidaMaterialAgregationService.ProcessoAssinaturaDocumentoOrigemRepository.ListarProcessosAssinaturaPorOrigem(saidaMaterial.Id.ToString(), processoAssinaturaDocumentoOrigemNome, cancellationToken);

                if (processoList.IsFailure)
                    return processoList.Failure;

                var alteracaoStatus = await AtualizarStatusEEmitirEventoAlteracaoParaCancelamentoDaSolicitacao(processoList, cancellationToken);
                if (alteracaoStatus.IsFailure)
                    return alteracaoStatus.Failure;

                transaction.Commit();

                return saidaMaterial;
            }
        }

        private async Task<TryException<Return>> AtualizarStatusEEmitirEventoAlteracaoParaCancelamentoDaSolicitacao(TryException<IEnumerable<ProcessoAssinaturaDocumentoOrigemModel>> listaProcessos, CancellationToken cancellationToken)
        {
            var processoListId = listaProcessos.Success?.Select(pado => pado.ProcessoAssinaturaDocumentoId);

            if (processoListId?.Any() != null)
            {
                var atualizarProcessosList = await _solicitacaoSaidaMaterialAgregationService.ProcessoAssinaturaDocumentoService.AtualizarStatusProcessos(processoListId, StatusAssinaturaDocumento.Cancelado, cancellationToken);
                if (atualizarProcessosList.IsFailure)
                    return atualizarProcessosList.Failure;

                foreach (var pad in atualizarProcessosList.Success)
                {
                    var emitirAlteracaoStatusDocumento = await _solicitacaoSaidaMaterialAgregationService.GdocsWorkerExternalService.EmitirAlteracaoStatusDocumento(pad.Id);
                    if (emitirAlteracaoStatusDocumento.IsFailure)
                        return emitirAlteracaoStatusDocumento.Failure;
                }
            }

            return Return.Empty;
        }

        private async Task<TryException<Return>> EditarBinario(SolicitacaoSaidaMaterialModel SolicitacaoMaterial, int padId, CancellationToken cancellationToken)
        {

            var assinatura = new AssinaturaModel()
            {
                Arquivos = new List<AssinaturaArquivoModel> { new AssinaturaArquivoModel() {
                        BinarioId = SolicitacaoMaterial.BinarioId,
                        Ordem = 0
                    }}
            };

            var arquivo = assinatura.Arquivos.First();
            assinatura.Arquivos = new List<AssinaturaArquivoModel>() { arquivo };

            var arquivoSalvo = await _arquivoRepository.Salvar(padId, assinatura.Arquivos, cancellationToken, SolicitacaoMaterial.PodeEditar());
            if (arquivoSalvo.IsFailure)
                return arquivoSalvo.Failure;



            return Return.Empty;
        }

        }
}
