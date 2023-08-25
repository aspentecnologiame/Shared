using ICE.GDocs.Common.Core.Exceptions;
using ICE.GDocs.Domain.ExternalServices;
using ICE.GDocs.Domain.GDocs.Repositories.SolicitacaoSaidaMaterial;
using ICE.GDocs.Domain.GDocs.Services.SolicitacaoSaidaMaterial;
using ICE.GDocs.Domain.Repositories;
using ICE.GDocs.Domain.Repositories.ProcessoAssinaturaDocumento;
using ICE.GDocs.Domain.Services;
using ICE.GDocs.Infra.CrossCutting.Models;
using ICE.GDocs.Infra.CrossCutting.Models.Enums;
using Microsoft.Extensions.Configuration;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

namespace ICE.GDocs.Application.GDocs.SolicitacaoSaidaMaterial
{
    internal class SolicitacaoSaidaMaterialAppService : ISolicitacaoSaidaMaterialAppService
    {
        private readonly ISolicitacaoSaidaMaterialAgregationAppService _solicitacaoSaidaMaterialAgregationAppService;
        private readonly ISolicitacaoSaidaMaterialService _solicitacaoSaidaMaterialService;
        private readonly ISolicitacaoSaidaMaterialAcaoService _solicitacaoSaidaMaterialAcaoService;
        private readonly IProcessoAssinaturaDocumentoOrigemRepository _processoAssinaturaDocumentoOrigemRepository;
        private readonly IActiveDirectoryExternalService _activeDirectoryExternalService;
        private readonly ISolicitacaoSaidaMaterialRepository _solicitacaoSaidaMaterialRepository;
        private readonly IBinarioRepository _binarioRepository;

        private const string REGISTRO_NAO_ENCONTRADO = "documentofi347:obterbase64dopdf:saidamaterial:naoencontrado";
        private const string PDF_NAO_ENCONTRADO = "documentofi347:obterbase64dopdf:saidamaterial:pdf:naoencontrado";
        private const string CANCELAR_CODERRO = "documentofi347:cancelar";

        public SolicitacaoSaidaMaterialAppService(ISolicitacaoSaidaMaterialService solicitacaoSaidaMaterialService,
            ISolicitacaoSaidaMaterialAcaoService solicitacaoSaidaMaterialAcaoService,
            IProcessoAssinaturaDocumentoOrigemRepository processoAssinaturaDocumentoOrigemRepository,
            IActiveDirectoryExternalService activeDirectoryExternalService,
            ISolicitacaoSaidaMaterialRepository solicitacaoSaidaMaterialRepository,
            IBinarioRepository binarioRepository,
            ISolicitacaoSaidaMaterialAgregationAppService solicitacaoSaidaMaterialAgregationAppService)
        {
            _solicitacaoSaidaMaterialService = solicitacaoSaidaMaterialService;
            _solicitacaoSaidaMaterialAcaoService = solicitacaoSaidaMaterialAcaoService;
            _solicitacaoSaidaMaterialAgregationAppService = solicitacaoSaidaMaterialAgregationAppService;
            _processoAssinaturaDocumentoOrigemRepository = processoAssinaturaDocumentoOrigemRepository;
            _activeDirectoryExternalService = activeDirectoryExternalService;
            _solicitacaoSaidaMaterialRepository = solicitacaoSaidaMaterialRepository;
            _binarioRepository = binarioRepository;
        }

        public async Task<TryException<SolicitacaoSaidaMaterialModel>> ObterPorId(int id, CancellationToken cancellationToken)
        => await _solicitacaoSaidaMaterialService.ObterPorId(id, cancellationToken);


        public async Task<TryException<SolicitacaoSaidaMaterialModel>> ObterMaterialComItensPorId(int id, CancellationToken cancellationToken)
             => await _solicitacaoSaidaMaterialService.ObterMaterialComItensPorId(id, cancellationToken);

        public async Task<TryException<SolicitacaoSaidaMaterialModel>> Atualizar(SolicitacaoSaidaMaterialModel solicitacaoSaidaMaterialModel, CancellationToken cancellationToken)
        => await _solicitacaoSaidaMaterialService.Atualizar(solicitacaoSaidaMaterialModel, cancellationToken);

        public async Task<TryException<SolicitacaoSaidaMaterialModel>> Inserir(SolicitacaoSaidaMaterialModel solicitacaoSaidaMaterialModel, CancellationToken cancellationToken)
        {
            var adicionar = await _solicitacaoSaidaMaterialService.Adicionar(solicitacaoSaidaMaterialModel, cancellationToken);

            if (adicionar.IsFailure)
                return adicionar.Failure;

            return adicionar.Success;
        }

        public async Task<TryException<int>> EnviarParaAssinatura(SolicitacaoSaidaMaterialModel documento, CancellationToken cancellationToken)
        {
            await _solicitacaoSaidaMaterialAgregationAppService.LogService.AdicionarRastreabilidade(documento, $"Iniciando envio da FI {documento.Id} para assinatura.");

            var processoAssinaturaOrigemNome = _solicitacaoSaidaMaterialAgregationAppService.Configuration.GetValue("DocumentoFI347:ProcessoAssinaturaDocumentoOrigemNome", string.Empty);

            var processoAssinaturaOrigem = await _processoAssinaturaDocumentoOrigemRepository.ListarPorProcessoAssinaturaDocumentoOrigemIdENome(documento.Id.ToString(), processoAssinaturaOrigemNome, cancellationToken);
            if (processoAssinaturaOrigem.IsFailure)
                return processoAssinaturaOrigem.Failure;

            if (processoAssinaturaOrigem.Success != null)
            {
                await _solicitacaoSaidaMaterialAgregationAppService.LogService.AdicionarRastreabilidade(documento, $"FI {documento.Id} já havia sido enviado para assinatura. ProcessoAssinaturaDocumentoId: {processoAssinaturaOrigem.Success.ProcessoAssinaturaDocumentoId}");
                return processoAssinaturaOrigem.Success.ProcessoAssinaturaDocumentoId;
            }

            var titulo = string.Format(_solicitacaoSaidaMaterialAgregationAppService.Configuration.GetValue("DocumentoFI347:ProcessoAssinaturaTitulo", string.Empty), documento.Numero);
            var nomeDocumento = _solicitacaoSaidaMaterialAgregationAppService.Configuration.GetValue("DocumentoFI347:ProcessoAssinaturaNomeDocumento", string.Empty);

            var categoriaId = documento.FlgRetorno ?
                _solicitacaoSaidaMaterialAgregationAppService.Configuration.GetValue("DocumentoFI347:ProcessoAssinaturaCategoriaIdComRetorno", 31) : 
                _solicitacaoSaidaMaterialAgregationAppService.Configuration.GetValue("DocumentoFI347:ProcessoAssinaturaCategoriaIdSemRetorno", 32);

            var enviarAssinatura = await _solicitacaoSaidaMaterialService.EnviarParaAssinatura(documento, processoAssinaturaOrigemNome, titulo, nomeDocumento, categoriaId, cancellationToken);

            if (enviarAssinatura.IsFailure)
                return enviarAssinatura.Failure;

            await _solicitacaoSaidaMaterialAgregationAppService.LogService.AdicionarRastreabilidade(documento, $"Finalizou envio da FI {documento.Id} para assinatura. ProcessoAssinaturaDocumentoId: {enviarAssinatura.Success}");

            return enviarAssinatura.Success;
        }

        public async Task<TryException<Return>> Excluir(int id, CancellationToken cancellationToken)
            => await _solicitacaoSaidaMaterialService.Excluir(id, cancellationToken);

        public async Task<TryException<IEnumerable<SolicitacaoSaidaMaterialModel>>> ConsultarSolicitacoesDeSaidaMateriaisPorFiltro(
                  SolicitacaoSaidaMaterialFilterModel filtro,
                  CancellationToken cancellationToken
              )
        {
            await _solicitacaoSaidaMaterialAgregationAppService.LogService.AdicionarRastreabilidade(filtro, "Iniciando a busca de solicitações de saída de material com o filtro Selecionado.");

            filtro.Ordenacao = await this.DefinirOrdenacaoRelatorioSaidaMaterial(filtro.Ordenacao);
            var response = await _solicitacaoSaidaMaterialRepository.Listar(filtro, cancellationToken);

            if (response.IsFailure)
                return response.Failure;

            await _solicitacaoSaidaMaterialAgregationAppService.LogService.AdicionarRastreabilidade(filtro, $"{response.Success.Count()} resultado encontrado.");

            var usuarios = _activeDirectoryExternalService.GetActiveDirectoryUsers(string.Empty, response.Success.Select(x => x.GuidResponsavel).Distinct().AsList());

            if (usuarios.IsFailure)
                return usuarios.Failure;

            var results = response.Success.Join(usuarios.Success,
                      d => d.GuidResponsavel,
                      u => u.Guid,
                      (mat, usu) => mat.DefinirNomeResponsavel(usu.Nome)
                    );

            await _solicitacaoSaidaMaterialAgregationAppService.LogService.AdicionarRastreabilidade(filtro, $"Total de {results.Count()} solicitações de saída de material após junção com os autores ad.");

            return results?.ToCollection();
        }

        public async Task<TryException<string>> ObterBase64DoPdf(int saidaMaterialId, CancellationToken cancellationToken)
        {
            var documentoObterPorId = await ObterPorId(saidaMaterialId, cancellationToken);

            if (documentoObterPorId.IsFailure)
                return documentoObterPorId.Failure;

            if (documentoObterPorId.Success == null)
                return new BusinessException(REGISTRO_NAO_ENCONTRADO, $"Não foi encontrado nenhuma saída de material com o id {saidaMaterialId}.");

            if (!documentoObterPorId.Success.ExisteBinario())
                return new BusinessException(REGISTRO_NAO_ENCONTRADO, $"Documento não está mais disponível para visualização. Temporalidade expirada.");

            var documento = documentoObterPorId.Success;

            var binario = await _binarioRepository.ObterPorId(documento.BinarioId, cancellationToken);

            if (binario.IsFailure)
                return binario.Failure;

            if (binario.Success == null)
                return new BusinessException(PDF_NAO_ENCONTRADO, "PDF não encontrado.");

            return Convert.ToBase64String(binario.Success);
        }

        public async Task<TryException<SolicitacaoSaidaMaterialModel>> CancelarOuSolicitarCiencia(int solicitacaoSaidaMaterialId, string motivo, UsuarioModel usuarioModel, bool permiteCancelarTodos, CancellationToken cancellationToken)
        {
            TryException<SolicitacaoSaidaMaterialModel> result;

            var obterSaidaMaterialPorId = await ObterMaterialComItensPorId(solicitacaoSaidaMaterialId, cancellationToken);

            if (obterSaidaMaterialPorId.IsFailure)
                return obterSaidaMaterialPorId.Failure;

            if (obterSaidaMaterialPorId.Success == null)
                return new BusinessException(CANCELAR_CODERRO, "Solicitação de saída de material não encontrada.");

            var saidaMaterial = obterSaidaMaterialPorId.Success;

            if (saidaMaterial.Status == Infra.CrossCutting.Models.Enums.SolicitacaoSaidaMaterialStatus.PendenteSaida.ToString() && string.IsNullOrEmpty(motivo))
                return new BusinessException(CANCELAR_CODERRO, "Não é possível cancelar solicitação de saída de material com status de saída pendente, sem informar um motivo");

            if (saidaMaterial.Status == Infra.CrossCutting.Models.Enums.SolicitacaoSaidaMaterialStatus.PendenteSaida.GetDescription())
                result = await EnviarParaCiencia(saidaMaterial, motivo, usuarioModel, cancellationToken);
            else
                result = await Cancelar(saidaMaterial, usuarioModel, permiteCancelarTodos, cancellationToken);

            return result;
        }

        private async Task<TryException<SolicitacaoSaidaMaterialModel>> EnviarParaCiencia(SolicitacaoSaidaMaterialModel saidaMaterial, string motivo, UsuarioModel usuarioModel, CancellationToken cancellationToken)
        {
            var solicitacaoSaidaMaterialAcaoModel = new SolicitacaoSaidaMaterialAcaoModel
            {
                IdSolicitacaoSaidaMaterial = saidaMaterial.Id,
                IdSaidaMaterialTipoAcao = (int)SaidaMaterialTipoAcao.SolicitacaoCancelamento, 
                Observacao = motivo,
                FlgAtivo = true,
                DataCriacao = DateTime.Now,
                DataAtualizacao = DateTime.Now
            };

            saidaMaterial.ItemMaterial.ForEach((item) =>
            {
                var solicitacaoSaidaMaterialAcaoItem = new SolicitacaoSaidaMaterialAcaoItemModel(item.IdSolicitacaoSaidaMaterialItem)
                {
                    FlgAtivo = true,
                    DataCriacao = DateTime.Now,
                    DataAtualizacao = DateTime.Now
                };

                solicitacaoSaidaMaterialAcaoModel.AcaoItems.Add(solicitacaoSaidaMaterialAcaoItem);
            });

            await _solicitacaoSaidaMaterialAcaoService.InserirAcao(usuarioModel.ActiveDirectoryId, solicitacaoSaidaMaterialAcaoModel, cancellationToken);

            return saidaMaterial;
        }

        private async Task<TryException<SolicitacaoSaidaMaterialModel>> Cancelar(SolicitacaoSaidaMaterialModel saidaMaterial, UsuarioModel usuarioModel, bool permiteCancelarTodos, CancellationToken cancellationToken)
        {
            await _solicitacaoSaidaMaterialAgregationAppService.LogService.AdicionarRastreabilidade(saidaMaterial.Id, "Iniciando cancelamento da saída de material.");

            byte[] arquivo = default;
            if (saidaMaterial.StatusId == SolicitacaoSaidaMaterialStatus.EmConstrucao.ToInt32())
            {
                var binarioObterPorId = await _binarioRepository.ObterPorId(saidaMaterial.BinarioId, cancellationToken);
                if (binarioObterPorId.IsFailure)
                    return binarioObterPorId.Failure;

                arquivo = binarioObterPorId.Success;

                var carimboCancelado = Convert.FromBase64String(_solicitacaoSaidaMaterialAgregationAppService.Configuration.GetValue("Ice:PdfManager:Carimbos:Cancelado", string.Empty));
                arquivo = Framework.Pdf.PdfManager.CarimbarDocumento(arquivo, carimboCancelado);
            }

            var documentoCancelado = await _solicitacaoSaidaMaterialService.Cancelar(saidaMaterial, usuarioModel, permiteCancelarTodos, cancellationToken, arquivo);

            if (documentoCancelado.IsFailure)
                return documentoCancelado.Failure;

            await _solicitacaoSaidaMaterialAgregationAppService.LogService.AdicionarRastreabilidade(saidaMaterial.Id, "Solicitação de saída de material - Documento foi salvo.");

            return documentoCancelado.Success;
        }

        public async Task<TryException<IEnumerable<DocumentoFI347StatusModel>>> ListarStatusMaterial(CancellationToken cancellationToken) =>
            await  _solicitacaoSaidaMaterialRepository.ListarStatusMaterial(cancellationToken);

        private async Task<SolicitacaoSaidaMaterialOrdenacaoModel> DefinirOrdenacaoRelatorioSaidaMaterial(SolicitacaoSaidaMaterialOrdenacaoModel ordenacao)
        {
            ordenacao = ordenacao ?? new SolicitacaoSaidaMaterialOrdenacaoModel();
            switch (ordenacao.Active)
            {
                case "retorno":
                    ordenacao.OrderBy = $"ssm.ssm_dat_retorno {ordenacao.Direction}, ssm.ssm_num asc";
                    break;

                case "numero":
                    ordenacao.OrderBy = $"ssm.ssm_num {ordenacao.Direction}";
                    break;

                case "tipoSaida":
                    ordenacao.OrderBy = $"tiposaida {ordenacao.Direction}, ssm.ssm_num asc";
                    break;

                case "destino":
                    ordenacao.OrderBy = $"ssm.ssm_des_destino {ordenacao.Direction}, ssm.ssm_num asc";
                    break;

                case "status":
                    ordenacao.OrderBy = $"stsm.stsm_des {ordenacao.Direction}, ssm.ssm_num asc";
                    break;

                case "dataAcaoSaida":
                    ordenacao.OrderBy = $"ssma.ssma_dat_acao {ordenacao.Direction}, ssm.ssm_num asc";
                    break;

                default:
                    ordenacao.Direction = "Desc";
                    ordenacao.OrderBy = $"ssm.ssm_dat_cricao {ordenacao.Direction}, ssm.ssm_num asc";
                    break;
            }

            return await Task.FromResult(ordenacao);
        }
        
    }
}
