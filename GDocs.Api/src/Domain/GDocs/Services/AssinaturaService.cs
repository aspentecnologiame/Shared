using ICE.GDocs.Common.Core.Exceptions;
using ICE.GDocs.Domain.Core.Services;
using ICE.GDocs.Domain.Core.Uow;
using ICE.GDocs.Domain.ExternalServices;
using ICE.GDocs.Domain.GDocs.Repositories.ProcessoAssinaturaDocumento;
using ICE.GDocs.Domain.Repositories;
using ICE.GDocs.Domain.Repositories.ProcessoAssinaturaDocumento;
using ICE.GDocs.Infra.CrossCutting.Models;
using ICE.GDocs.Infra.CrossCutting.Models.Enums;
using Microsoft.Extensions.Configuration;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

namespace ICE.GDocs.Domain.Services
{
    internal class AssinaturaService : DomainService, IAssinaturaService
    {
        private readonly IAssinaturaAgregationService _assinaturaAgregationService;
        private readonly IAssinaturaArmazenadaUsuarioRepository _assinaturaArmazenadaUsuarioRepository;
        private readonly IInformacaoRepository _informacaoRepository;
        private readonly IPassoUsuarioRepository _passoUsuarioRepository;
        private readonly IConfiguracaoRepository _configuracaoRepository;
        private readonly IProcessoAssinaturaArquivoRepository _processoAssinaturaArquivoRepository;
        

        public AssinaturaService(
            IAssinaturaAgregationService assinaturaAgregationService,
            IAssinaturaArmazenadaUsuarioRepository assinaturaArmazenadaUsuarioRepository,
            IInformacaoRepository informacaoRepository,
            IPassoUsuarioRepository passoUsuarioRepository,
            IConfiguracaoRepository configuracaoRepository,
            IProcessoAssinaturaArquivoRepository processoAssinaturaArquivoRepository,
            IUnitOfWork unitOfWork) : base(unitOfWork)
        {
            _assinaturaAgregationService = assinaturaAgregationService;
            _assinaturaArmazenadaUsuarioRepository = assinaturaArmazenadaUsuarioRepository;
            _informacaoRepository = informacaoRepository;
            _passoUsuarioRepository = passoUsuarioRepository;
            _configuracaoRepository = configuracaoRepository;
            _processoAssinaturaArquivoRepository = processoAssinaturaArquivoRepository;    
        }

        public async Task<TryException<ArquivoModel>> Converter(string extensao, ArquivoModel arquivo, CancellationToken cancellationToken)
            => await _assinaturaAgregationService.DocToolsExternalService.Converter(extensao, arquivo, cancellationToken);

        public async Task<TryException<AssinaturaModel>> SalvarProcesso(AssinaturaModel assinatura, string uploadBasePath, CancellationToken cancellationToken)
        {
            await _assinaturaAgregationService.LogService.AdicionarRastreabilidade(assinatura, "Iniciando gravação de processo de assinatura.");

            var configuracaoCategoria = await _configuracaoRepository.ObterConfiguracao(_assinaturaAgregationService.Configuration.GetValue("ChaveConfiguracao:Categoria", "configCategorias"), cancellationToken);
            if (configuracaoCategoria.IsFailure)
                return configuracaoCategoria.Failure;

            var configCategorias = Newtonsoft.Json.JsonConvert.DeserializeObject<System.Collections.Generic.List<ConfiguracaoCategoriaModel>>(configuracaoCategoria.Success.Valor);

            using (var transaction = _unitOfWork.BeginTransaction())
            {
                await DefinirStatus(assinatura.Informacoes);

                var processoAssinatura = await _assinaturaAgregationService.ProcessoAssinaturaDocumentoService.Salvar(assinatura, uploadBasePath, configCategorias, cancellationToken);
                if (processoAssinatura.IsFailure)
                    return processoAssinatura.Failure;

                await _assinaturaAgregationService.LogService.AdicionarRastreabilidade(assinatura, "IProcesso de assinatura foi adicionado com sucesso.");

                var emitirAlteracaoStatusDocumento = await _assinaturaAgregationService.GdocsWorkerExternalService.EmitirAlteracaoStatusDocumento(processoAssinatura.Success.Informacoes.Id);
                if (emitirAlteracaoStatusDocumento.IsFailure)
                    return emitirAlteracaoStatusDocumento.Failure;

                await _assinaturaAgregationService.LogService.AdicionarRastreabilidade(processoAssinatura.Success.Informacoes.Id, "Evento de alteração de status foi emitido com sucesso.");

                transaction.Commit();

                return processoAssinatura;
            }
        }

        public async Task<TryException<Return>> SalvarFareiEnvio(AssinaturaModel assinaturaModel, Guid usuarioId, bool fareiEnvio, CancellationToken cancellationToken)
        {

            if (assinaturaModel.Passos.Itens.SelectMany(passo => passo.Usuarios).FirstOrDefault(x => x.FlagFareiEnvio) != null && fareiEnvio)
                return new BusinessException("Já existe um usuário marcado para fazer o envio.");

            if (assinaturaModel.Passos.Itens.SelectMany(passo => passo.Usuarios).FirstOrDefault(x => x.Guid.Equals(usuarioId) || x.GuidRepresentante.Equals(usuarioId)) == null)
                return new BusinessException("Usuario não encontrado.");

            var PassoUsuario = assinaturaModel.Passos.Itens.SelectMany(passo => passo.Usuarios).First(x => x.Guid.Equals(usuarioId) || x.GuidRepresentante.Equals(usuarioId)).Id;

            using (var transaction = _unitOfWork.BeginTransaction())
            {
                var AtualizarFareiEnvio = await _passoUsuarioRepository.AtualizaStatusFareiEnvio(PassoUsuario, fareiEnvio, cancellationToken);
                if (AtualizarFareiEnvio.IsFailure)
                    return AtualizarFareiEnvio.Failure;

                transaction.Commit();

            }

            if (assinaturaModel.Passos.Itens.SelectMany(passo => passo.Usuarios).Count(x => x.Status != StatusAssinaturaDocumentoPassoUsuario.Assinado) > 1 && fareiEnvio)
                return new BusinessException("Existem usuários pendentes para realizar a assinatura. Você receberá um e-mail no final do processo com documento assinado.");

            return Return.Empty;
        }

        public async Task<TryException<AssinaturaInformacoesModel>> CancelarProcesso(AssinaturaInformacoesModel model, CancellationToken cancellationToken)
        {
            using (var transaction = _unitOfWork.BeginTransaction())
            {
                await _assinaturaAgregationService.LogService.AdicionarRastreabilidade(model, "Iniciando cancelamento do processo de assinatura");

                var cancelarProcesso = await _assinaturaAgregationService.ProcessoAssinaturaDocumentoService.CancelarProcesso(model, cancellationToken);

                if (cancelarProcesso.IsFailure)
                    return cancelarProcesso.Failure;

                await _assinaturaAgregationService.LogService.AdicionarRastreabilidade(model, "Processo de assinatura cancelado com sucesso.");

                transaction.Commit();

                return cancelarProcesso;
            }
        }

        public async Task<TryException<Return>> SalvarArquivos(IEnumerable<AssinaturaArquivoModel> assinaturaArquivoModel, string uploadBasePath, CancellationToken cancellationToken)
        {
            using (var transaction = _unitOfWork.BeginTransaction())
            {
                if (assinaturaArquivoModel == null || !assinaturaArquivoModel.Any())
                    return new BusinessException("Adicione ao menos um documento");

                var assinatura = new AssinaturaModel()
                {
                    Informacoes = new AssinaturaInformacoesModel() { Id = assinaturaArquivoModel.FirstOrDefault().ProcessoAssinaturaDocumentoId },
                    Arquivos = assinaturaArquivoModel
                };

                await DefinirStatus(assinatura.Informacoes);

                var atualizouStatus = await _assinaturaAgregationService.ProcessoAssinaturaDocumentoService.AtualizarStatusProcessos(new List<int> { assinatura.Informacoes.Id }, assinatura.Informacoes.Status, cancellationToken);
                if (atualizouStatus.IsFailure)
                    return atualizouStatus.Failure;

                var processoAssinatura = await _assinaturaAgregationService.ProcessoAssinaturaDocumentoService.CriarBinariosEIncluirArquivos(assinatura, uploadBasePath, cancellationToken);
                if (processoAssinatura.IsFailure)
                    return processoAssinatura.Failure;

                var emitirAlteracaoStatusDocumento = await _assinaturaAgregationService.GdocsWorkerExternalService.EmitirAlteracaoStatusDocumento(processoAssinatura.Success.Informacoes.Id);
                if (emitirAlteracaoStatusDocumento.IsFailure)
                    return emitirAlteracaoStatusDocumento.Failure;

                transaction.Commit();

                return Return.Empty;
            }
        }

        public async Task<TryException<Return>> Assinar(Guid usuarioAdGuid, AssinaturaPassoItemAssinarRejeitarModel assinaturaPassoItemAssinarModel, string uploadPathBase, CancellationToken cancellationToken)
        {
            using (var transaction = _unitOfWork.BeginTransaction())
            {
                var listarPassosUsuarioPorPadId = await _passoUsuarioRepository.ListarPorPadId(assinaturaPassoItemAssinarModel.ListaDeprocessoAssinaturaDocumentoId, usuarioAdGuid, cancellationToken);
                if (listarPassosUsuarioPorPadId.IsFailure)
                    return listarPassosUsuarioPorPadId.Failure;

                var necessidadeDeAssinaturaArmazenada = await ValidarNecessidadeDeAssinaturaArmazenada(
                    listarPassosUsuarioPorPadId.Success,
                    usuarioAdGuid,
                    cancellationToken
                    );

                if (necessidadeDeAssinaturaArmazenada.IsFailure)
                    return necessidadeDeAssinaturaArmazenada.Failure;

                var validarStatusPad = await ValidarStatusPadAssinarRejeitar(assinaturaPassoItemAssinarModel.ListaDeprocessoAssinaturaDocumentoId, cancellationToken);
                if (validarStatusPad.IsFailure)
                    return validarStatusPad.Failure;

                await _assinaturaAgregationService.LogService.AdicionarRastreabilidade(assinaturaPassoItemAssinarModel.ListaDeprocessoAssinaturaDocumentoId, "Status é válido para assinatura dos PadIds");

                var validarUsuarioJaAssinouProcesso = await ValidarSeOUsuarioJaAssinouAlgumDosProcessos(usuarioAdGuid, assinaturaPassoItemAssinarModel.ListaDeprocessoAssinaturaDocumentoId, StatusAssinaturaDocumentoPassoUsuario.AguardandoAssinatura, cancellationToken);
                if (validarUsuarioJaAssinouProcesso.IsFailure)
                    return validarUsuarioJaAssinouProcesso.Failure;

                await _assinaturaAgregationService.LogService.AdicionarRastreabilidade(assinaturaPassoItemAssinarModel.ListaDeprocessoAssinaturaDocumentoId, "Usuário ainda não assinou o processo. Tudo Ok para seguir.");

                var assinar = await _assinaturaAgregationService.ProcessoAssinaturaDocumentoService.AssinarRejeitar(
                    usuarioAdGuid,
                    assinaturaPassoItemAssinarModel.ListaDeprocessoAssinaturaDocumentoId,
                    StatusAssinaturaDocumentoPassoUsuario.Assinado,
                    assinaturaPassoItemAssinarModel.Justificativa == string.Empty ? null : assinaturaPassoItemAssinarModel.Justificativa,
                    cancellationToken);

                if (assinar.IsFailure)
                    return assinar.Failure;

                await _assinaturaAgregationService.LogService.AdicionarRastreabilidade(new { usuarioAdGuid, assinaturaPassoItemAssinarModel.ListaDeprocessoAssinaturaDocumentoId }, "PadIds assinados com sucesso");

                var processarPassosComAssinaturaDigital = await ProcessarPassosComAssinaturaDigital(
                    assinaturaPassoItemAssinarModel,
                    listarPassosUsuarioPorPadId.Success,
                    uploadPathBase,
                    cancellationToken);

                if (processarPassosComAssinaturaDigital.IsFailure)
                    return processarPassosComAssinaturaDigital.Failure;

                foreach (var processoAssinaturaDocumentoId in assinaturaPassoItemAssinarModel.ListaDeprocessoAssinaturaDocumentoId)
                {
                    var emitirAlteracaoStatusDocumento = await _assinaturaAgregationService.GdocsWorkerExternalService.EmitirAlteracaoStatusDocumento(processoAssinaturaDocumentoId);
                    if (emitirAlteracaoStatusDocumento.IsFailure)
                        return emitirAlteracaoStatusDocumento.Failure;
                }

                await _assinaturaAgregationService.LogService.AdicionarRastreabilidade(assinaturaPassoItemAssinarModel.ListaDeprocessoAssinaturaDocumentoId, "PadIds enviados para o worker com sucesso");

                transaction.Commit();

                return Return.Empty;
            }
        }

        private async Task<TryException<Return>> ProcessarPassosComAssinaturaDigital(
            AssinaturaPassoItemAssinarRejeitarModel assinaturaPassoItemAssinarModel,
            IEnumerable<AssinaturaPassoAssinanteModel> passoList,
            string uploadPathBase,
            CancellationToken cancellationToken)
        {
            var temAlgumPassoUsuarioDeAlgumProcessoComAssinaturaDigital = passoList.Any(padpu => padpu.AssinarCertificadoDigital);

            if (temAlgumPassoUsuarioDeAlgumProcessoComAssinaturaDigital)
            {
                if (assinaturaPassoItemAssinarModel.UploadDocumentoComAssinaturaDigital == null)
                    return new BusinessException("Você está assinando um documento digitalmente. É necessário realizar upload do arquivo assinado.");

                foreach (var passoComAssinaturaDigital in passoList.Where(padpu => padpu.AssinarCertificadoDigital))
                {
                    var arquivoFisicoComAssinaturaDigital = File.ReadAllBytes(Path.Combine(uploadPathBase, assinaturaPassoItemAssinarModel.UploadDocumentoComAssinaturaDigital.NomeSalvo));
                    var binarioComAssinaturaDigital = await _processoAssinaturaArquivoRepository.BinarioRepository
                        .Inserir(arquivoFisicoComAssinaturaDigital, cancellationToken);

                    if (binarioComAssinaturaDigital.IsFailure)
                        return binarioComAssinaturaDigital.Failure;

                    var inativarAquivoProcessoAssinaturaCorrente = await _processoAssinaturaArquivoRepository.ArquivoRepository
                        .Inativar(passoComAssinaturaDigital.ProcessoAssinaturaArquivoCorrenteId, cancellationToken);

                    if (inativarAquivoProcessoAssinaturaCorrente.IsFailure)
                        return inativarAquivoProcessoAssinaturaCorrente.Failure;

                    var arquivoComAssinaturaDigital = new AssinaturaArquivoModel
                    {
                        ProcessoAssinaturaDocumentoId = passoComAssinaturaDigital.ProcessoAssinaturaId,
                        BinarioId = binarioComAssinaturaDigital.Success
                    };

                    var associarArquivoComAssinaturaDigitalAoProcesso = await _processoAssinaturaArquivoRepository.ArquivoRepository
                        .Salvar(passoComAssinaturaDigital.ProcessoAssinaturaId, new List<AssinaturaArquivoModel> { arquivoComAssinaturaDigital }, cancellationToken);

                    if (associarArquivoComAssinaturaDigitalAoProcesso.IsFailure)
                        return associarArquivoComAssinaturaDigitalAoProcesso.Failure;
                }
            }

            return Return.Empty;
        }

        private async Task<TryException<Return>> ValidarNecessidadeDeAssinaturaArmazenada(
            IEnumerable<AssinaturaPassoAssinanteModel> passoList,
            Guid usuarioAd,
            CancellationToken cancellationToken)
        {
            var precisaDeAssinaturaArmazenadaParaAssinar = !passoList.All(padpu => padpu.AssinarCertificadoDigital);

            if (precisaDeAssinaturaArmazenadaParaAssinar)
            {
                var assinaturaArmazenada = await _assinaturaArmazenadaUsuarioRepository.ListarPorUsuario(usuarioAd, cancellationToken);
                if (assinaturaArmazenada.IsFailure)
                    return assinaturaArmazenada.Failure;

                if (assinaturaArmazenada.Success == null)
                    return new BusinessException("Você não possui uma assinatura armazenada no sistema. Cadastre uma assinatura clicando em 'Meus dados'.");

                await _assinaturaAgregationService.LogService.AdicionarRastreabilidade(assinaturaArmazenada.Success.Guid, "Usuário tem uma assinatura armazenada");
            }

            return Return.Empty;
        }

        public async Task<TryException<Return>> Rejeitar(Guid usuarioAdGuid, AssinaturaPassoItemAssinarRejeitarModel assinaturaPassoItemRejeitarModel, CancellationToken cancellationToken)
        {
            using (var transaction = _unitOfWork.BeginTransaction())
            {
                var validarStatusPad = await ValidarStatusPadAssinarRejeitar(assinaturaPassoItemRejeitarModel.ListaDeprocessoAssinaturaDocumentoId, cancellationToken);
                if (validarStatusPad.IsFailure)
                    return validarStatusPad.Failure;

                await _assinaturaAgregationService.LogService.AdicionarRastreabilidade(assinaturaPassoItemRejeitarModel.ListaDeprocessoAssinaturaDocumentoId, "Status é válido para assinatura dos PadIds");

                var validarUsuarioJaAssinouProcesso = await ValidarSeOUsuarioJaAssinouAlgumDosProcessos(usuarioAdGuid, assinaturaPassoItemRejeitarModel.ListaDeprocessoAssinaturaDocumentoId, StatusAssinaturaDocumentoPassoUsuario.AguardandoAssinatura, cancellationToken);
                if (validarUsuarioJaAssinouProcesso.IsFailure)
                    return validarStatusPad.Failure;

                await _assinaturaAgregationService.LogService.AdicionarRastreabilidade(assinaturaPassoItemRejeitarModel.ListaDeprocessoAssinaturaDocumentoId, "Usuário ainda não assinou o processo. Tudo Ok para seguir.");

                var rejeitar = await _assinaturaAgregationService.ProcessoAssinaturaDocumentoService.AssinarRejeitar(usuarioAdGuid, assinaturaPassoItemRejeitarModel.ListaDeprocessoAssinaturaDocumentoId, StatusAssinaturaDocumentoPassoUsuario.Rejeitado, assinaturaPassoItemRejeitarModel.Justificativa, cancellationToken);
                if (rejeitar.IsFailure)
                    return rejeitar.Failure;

                await _assinaturaAgregationService.LogService.AdicionarRastreabilidade(new { usuarioAdGuid, assinaturaPassoItemRejeitarModel }, "PadIds assinados com sucesso");

                foreach (var processoAssinaturaDocumentoId in assinaturaPassoItemRejeitarModel.ListaDeprocessoAssinaturaDocumentoId)
                {
                    var emitirAlteracaoStatusDocumento = await _assinaturaAgregationService.GdocsWorkerExternalService.EmitirAlteracaoStatusDocumento(processoAssinaturaDocumentoId);
                    if (emitirAlteracaoStatusDocumento.IsFailure)
                        return emitirAlteracaoStatusDocumento.Failure;
                }

                await _assinaturaAgregationService.LogService.AdicionarRastreabilidade(assinaturaPassoItemRejeitarModel.ListaDeprocessoAssinaturaDocumentoId, "PadIds enviados para o worker com sucesso");

                transaction.Commit();

                return Return.Empty;
            }
        }

        public async Task<TryException<Return>> AtualizarDestaqueProcesso(AssinaturaInformacoesModel assinaturaInformacoesModel, CancellationToken cancellationToken)
        {
            var informacoes = await _informacaoRepository.Salvar(assinaturaInformacoesModel, cancellationToken);

            if (informacoes.IsFailure)
                return informacoes.Failure;

            return Return.Empty;
        }

        private async Task<TryException<Return>> ValidarStatusPadAssinarRejeitar(IEnumerable<int> listaDeprocessoAssinaturaDocumentoId, CancellationToken cancellationToken)
        {
            var processoAssinatura = await _informacaoRepository.Listar(new AssinaturaInformacoesFilterModel()
            {
                Status = new List<int> { (int)StatusAssinaturaDocumentoPasso.EmAndamento },
                ListaDeprocessoAssinaturaDocumentoId = listaDeprocessoAssinaturaDocumentoId
            }, cancellationToken);
            if (processoAssinatura.IsFailure)
                return processoAssinatura.Failure;

            if (processoAssinatura.Success == null || processoAssinatura.Success.Count() != listaDeprocessoAssinaturaDocumentoId.Distinct().Count())
                return new BusinessException("Não é possível aprovar/rejeitar esse documento, pois ele não se encontra mais como pendente de aprovação.");

            return Return.Empty;
        }

        public async Task<TryException<Return>> ValidarSeOUsuarioJaAssinouAlgumDosProcessos(Guid usuarioAdGuid, IEnumerable<int> listaDeprocessoAssinaturaDocumentoId, StatusAssinaturaDocumentoPassoUsuario status, CancellationToken cancellationToken)
        {
            var assinantes = await _assinaturaAgregationService.ProcessoAssinaturaDocumentoService.ListarPorPadId(listaDeprocessoAssinaturaDocumentoId, cancellationToken);
            if (assinantes.IsFailure)
                return assinantes.Failure;

            if (assinantes.Success.Any(registro => registro.Status == status &&
                (registro.UsuarioAdAssinanteGuid == usuarioAdGuid || registro.UsuarioAdRepresentanteGuid == usuarioAdGuid)))
                return Return.Empty;

            return new BusinessException("Não é possível aprovar/rejeitar esse documento, pois ele não se encontra mais como pendente de aprovação.");
        }

        private async Task DefinirStatus(AssinaturaInformacoesModel informacoes)
        {
            if (informacoes.Status == StatusAssinaturaDocumento.EmConstrucao)
                informacoes.Status = StatusAssinaturaDocumento.NaoIniciado;

            await _assinaturaAgregationService.LogService.AdicionarRastreabilidade(informacoes, "Definindo status do processo de assinatura.");
        }
    }
}
