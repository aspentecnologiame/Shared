using ICE.GDocs.Common.Core.Exceptions;
using ICE.GDocs.Domain.Core.Services;
using ICE.GDocs.Domain.Core.Uow;
using ICE.GDocs.Domain.GDocs.Repositories.SaidaMaterialNotaFiscal;
using ICE.GDocs.Domain.GDocs.Repositories.SolicitacaoCiencia;
using ICE.GDocs.Domain.GDocs.Repositories.SolicitacaoSaidaMaterial;
using ICE.GDocs.Domain.GDocs.Services.SaidaMaterialNotaFiscal.Interface;
using ICE.GDocs.Infra.CrossCutting.Models;
using ICE.GDocs.Infra.CrossCutting.Models.Enums;
using ICE.GDocs.Infra.CrossCutting.Models.SaidaMaterialNotaFiscal;
using ICE.GDocs.Infra.CrossCutting.Models.SaidaMaterialNotaFiscal.Enums;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace ICE.GDocs.Domain.GDocs.Services.SaidaMaterialNotaFiscal.Service
{
    internal class SolicitacaoCienciaNotaFiscalService : DomainService, ISolicitacaoCienciaNotaFiscalService
    {
        private readonly ISaidaMaterialNotaFiscalCienciaRepository _saidaMaterialNotaFiscalCienciaRepository;
        private readonly ISaidaMaterialNotaFiscalRepository _saidaMaterialNotaFiscalRepository;
        private readonly ISaidaMaterialNotaFiscalItemRepository _saidaMaterialNotaFiscalItemRepository;
        private readonly ISaidaMaterialNotaFiscalAcaoService _saidaMaterialNotaFiscalAcaoService;
        public SolicitacaoCienciaNotaFiscalService(
            IUnitOfWork unitOfWork,
            ISaidaMaterialNotaFiscalRepository saidaMaterialNotaFiscalRepository,
            ISaidaMaterialNotaFiscalItemRepository saidaMaterialNotaFiscalItemRepository,
            ISaidaMaterialNotaFiscalAcaoService saidaMaterialNotaFiscalAcaoService,
            ISaidaMaterialNotaFiscalCienciaRepository saidaMaterialNotaFiscalCienciaRepository) : base(unitOfWork)
        {
            _saidaMaterialNotaFiscalRepository = saidaMaterialNotaFiscalRepository;
            _saidaMaterialNotaFiscalItemRepository = saidaMaterialNotaFiscalItemRepository;
            _saidaMaterialNotaFiscalAcaoService = saidaMaterialNotaFiscalAcaoService;
            _saidaMaterialNotaFiscalCienciaRepository = saidaMaterialNotaFiscalCienciaRepository;
        }
        public async Task<TryException<IEnumerable<ProcessoAssinaturaDocumentoModel>>> ListarCienciasPendentesParaAprovacaoPeloUsuario(Guid activeDirectoryId, CancellationToken cancellationToken) => await
            _saidaMaterialNotaFiscalCienciaRepository.ListarCienciasPendentesDeAprovacaoPeloUsuario(activeDirectoryId, cancellationToken);

        public async Task<TryException<SaidaMaterialNotaFiscalCienciaModel>> ObterCienciaNfPorId(int IdSolicitacaoCienciaNf, CancellationToken cancellationToken) => await
             _saidaMaterialNotaFiscalCienciaRepository.ObterCienciaPeloIdentificador(IdSolicitacaoCienciaNf, cancellationToken);

        public async Task<TryException<Return>> RegistroCienciaPorUsuario(SaidaMaterialNotaFiscalCienciaModel solicitacaoCienciaNfModel, Guid usuarioLogadoId, CancellationToken cancellationToken)
        {

            using (var transaction = _unitOfWork.BeginTransaction())
            {
                var CienciaAprovador = await _saidaMaterialNotaFiscalCienciaRepository.ObterAprovadoresPorUsuarioECiencia(solicitacaoCienciaNfModel.Id, usuarioLogadoId, cancellationToken);

                if (CienciaAprovador.IsFailure)
                    return CienciaAprovador.Failure;

                if (CienciaAprovador.Success == null)
                    return new BusinessException($"Não foi encontrado nenhuma aprovação pendente. Para solicitação de número: {solicitacaoCienciaNfModel.NumeroMaterial}");

                await _saidaMaterialNotaFiscalCienciaRepository.RegistroCiencia(solicitacaoCienciaNfModel, CienciaAprovador.Success.Id, cancellationToken);


                await ValidarAprovacaoFinal(solicitacaoCienciaNfModel, usuarioLogadoId, cancellationToken);

                transaction.Commit();
            }
            return Return.Empty;
        }


        private async Task<TryException<Return>> ValidarAprovacaoFinal(SaidaMaterialNotaFiscalCienciaModel solicitacaoCienciaNfModel, Guid usuarioLogadoId, CancellationToken cancellationToken)
        {
            var AprovadorCiencia = await _saidaMaterialNotaFiscalCienciaRepository.ObterAprovadoresPorCiencia(solicitacaoCienciaNfModel.Id, cancellationToken);
            if (AprovadorCiencia.Success.All(x => x.Aprovacao != null))
            {
                var registraHistorico = await _saidaMaterialNotaFiscalRepository.RegistroHistoricoProrogacao(solicitacaoCienciaNfModel, cancellationToken);
                if (registraHistorico.IsFailure) return registraHistorico.Failure;

                if (AprovadorCiencia.Success.Any(x => x.FlgRejeitado))
                {
                    solicitacaoCienciaNfModel.IdStatusCiencia = (int)Domain.GDocs.Enums.StatusCiencia.Rejeitado;
                    await _saidaMaterialNotaFiscalCienciaRepository.AtualizarCienciaStatus(solicitacaoCienciaNfModel, cancellationToken);
                    await _saidaMaterialNotaFiscalRepository.AtualizarPorIdStatus(solicitacaoCienciaNfModel.IdSaidaMaterialNotaFiscal, SaidaMaterialNotaFiscalStatus.EmAberto, cancellationToken);
                    return Return.Empty;
                }

                solicitacaoCienciaNfModel.IdStatusCiencia = (int)Domain.GDocs.Enums.StatusCiencia.Concluido;
                await _saidaMaterialNotaFiscalCienciaRepository.AtualizarCienciaStatus(solicitacaoCienciaNfModel, cancellationToken);

                if (solicitacaoCienciaNfModel.IdTipoCiencia == (int)TipoCiencia.BaixaSemRetorno)
                    return await ProcessarBaixaSemRetorno(solicitacaoCienciaNfModel, usuarioLogadoId, cancellationToken);

                var atualizarStatus = await _saidaMaterialNotaFiscalRepository.AtualizarDataStatus(solicitacaoCienciaNfModel, SaidaMaterialNotaFiscalStatus.EmAberto, cancellationToken);
                if (atualizarStatus.IsFailure) return atualizarStatus.Failure;
            }

            return Return.Empty;
        }
        private async Task<TryException<Return>> ProcessarBaixaSemRetorno(SaidaMaterialNotaFiscalCienciaModel solicitacaoCienciaNfModel, Guid usuarioLogadoId, CancellationToken cancellationToken)
        {
            var acaoModel = SaidaMaterialacaoManual(solicitacaoCienciaNfModel, SaidaMaterialNotaFiscalTipoAcao.BaixaMaterialSemRetorno);

            var registraAcao = await _saidaMaterialNotaFiscalAcaoService.InserirAcao(usuarioLogadoId, acaoModel, cancellationToken);
            if (registraAcao.IsFailure) return registraAcao.Failure;

            var ItensDaSolicitacaoMaterial = await _saidaMaterialNotaFiscalItemRepository.ObterPorIdSolicitacaoSaidaMaterial(solicitacaoCienciaNfModel.IdSaidaMaterialNotaFiscal, cancellationToken);
            if (ItensDaSolicitacaoMaterial.IsFailure) return ItensDaSolicitacaoMaterial.Failure;

            var QuantidadeItemMaterialBaixado = await _saidaMaterialNotaFiscalItemRepository.ObterQuantidadeItemComBaixa(solicitacaoCienciaNfModel.IdSaidaMaterialNotaFiscal, cancellationToken);
            if (QuantidadeItemMaterialBaixado.IsFailure) return QuantidadeItemMaterialBaixado.Failure;

            if (ItensDaSolicitacaoMaterial.Success.Count() == QuantidadeItemMaterialBaixado.Success)
            {
                await _saidaMaterialNotaFiscalRepository.AtualizarPorIdStatus(solicitacaoCienciaNfModel.IdSaidaMaterialNotaFiscal, SaidaMaterialNotaFiscalStatus.Concluida, cancellationToken);
                return Return.Empty;
            }

            await _saidaMaterialNotaFiscalRepository.AtualizarPorIdStatus(solicitacaoCienciaNfModel.IdSaidaMaterialNotaFiscal, SaidaMaterialNotaFiscalStatus.EmAberto, cancellationToken, true);

            return Return.Empty;
        }
        private SaidaMaterialNotaFiscalAcaoModel SaidaMaterialacaoManual(SaidaMaterialNotaFiscalCienciaModel solicitacaoCienciaNfModel, SaidaMaterialNotaFiscalTipoAcao status)
        {
            var acao = new SaidaMaterialNotaFiscalAcaoModel();
            acao.IdSaidaMaterialNotaFiscalTipoAcao = status.ToInt32();
            acao.IdSaidaMaterialNotaFiscal = solicitacaoCienciaNfModel.IdSaidaMaterialNotaFiscal;

            foreach (var item in solicitacaoCienciaNfModel.Itens)
                acao.AcaoItems.Add(new SaidaMaterialNotaFiscalAcaoItemModel(item.IdSaidaMaterialNotaFiscalItem));

            return acao;
        }

        public async Task<TryException<IEnumerable<CienciaUsuarioAprovacaoModel>>> ListarUsuarioAprovacao(int IdSolicitacaoCienciaNf, CancellationToken cancellationToken) =>
            await _saidaMaterialNotaFiscalCienciaRepository.ListarUsuarioAprovacaoPorId(IdSolicitacaoCienciaNf, cancellationToken);
    }
}
