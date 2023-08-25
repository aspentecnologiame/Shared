using ICE.GDocs.Domain.GDocs.Services.SolicitacaoSaidaMaterial;
using ICE.GDocs.Infra.CrossCutting.Models;
using ICE.GDocs.Infra.CrossCutting.Models.Enums;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace ICE.GDocs.Application.GDocs.SolicitacaoSaidaMaterial
{
    internal class SolicitacaoSaidaMaterialAcaoAppService : ISolicitacaoSaidaMaterialAcaoAppService
    {
        private readonly ISolicitacaoSaidaMaterialAcaoService _solicitacaoSaidaMaterialAcaoService;
        private readonly ISolicitacaoSaidaMaterialAppService _solicitacaoSaidaMaterialAppService;

        public SolicitacaoSaidaMaterialAcaoAppService(ISolicitacaoSaidaMaterialAcaoService solicitacaoSaidaMaterialAcaoService, ISolicitacaoSaidaMaterialAppService solicitacaoSaidaMaterialAppService)
        {
            _solicitacaoSaidaMaterialAcaoService = solicitacaoSaidaMaterialAcaoService;
            _solicitacaoSaidaMaterialAppService = solicitacaoSaidaMaterialAppService;
        }

        public async Task<TryException<int>> InserirAcao(Guid usuarioLogado, SolicitacaoSaidaMaterialAcaoModel solicitacaoSaidaMaterialAcaoModel, CancellationToken cancellationToken)
            => await _solicitacaoSaidaMaterialAcaoService.InserirAcao(usuarioLogado, solicitacaoSaidaMaterialAcaoModel, cancellationToken);

        public async Task<TryException<IEnumerable<SolicitacaoSaidaMaterialAcaoTipoModel>>> ListarAcaoTipo()
            => await _solicitacaoSaidaMaterialAcaoService.ListarAcaoTipo();

        public async Task<TryException<IEnumerable<HistoricoProggoracaoModel>>> ListarDatasDeProrrogacaoPorSolicitacaoDeSaidaDeMaterial(int idSolicitacaoSaidaMaterial, SaidaMaterialTipoAcao tipoAcao, CancellationToken cancellationToken)
            => await _solicitacaoSaidaMaterialAcaoService.ListarDatasDeProrrogacaoPorSolicitacaoDeSaidaDeMaterial(idSolicitacaoSaidaMaterial,tipoAcao, cancellationToken);

        public async Task<TryException<IEnumerable<HistoricoProggoracaoModel>>> ListarHistoricoBaixaSemRetorno(int idSolicitacaoSaidaMaterial, CancellationToken cancellationToken)
                   => await _solicitacaoSaidaMaterialAcaoService.ListarHistoricoBaixaSemRetorno(idSolicitacaoSaidaMaterial, cancellationToken);


        public async Task<TryException<IEnumerable<SolicitacaoSaidaMaterialAcaoModel>>> LitarAcaoSaidaEhRetorno(int idSolicitacaoSaidaMaterial, CancellationToken cancellationToken) =>
            await _solicitacaoSaidaMaterialAcaoService.LitarAcaoSaidaEhRetorno(idSolicitacaoSaidaMaterial, cancellationToken);

        public async Task<TryException<HistoricoResponseModel>> ObterHistoricoMaterial(int idSolicitacaoSaidaMaterial, CancellationToken cancellationToken)
        {

            var saidaMaterial = await _solicitacaoSaidaMaterialAppService.ObterMaterialComItensPorId(idSolicitacaoSaidaMaterial, cancellationToken);
            if (saidaMaterial.IsFailure)
                return saidaMaterial.Failure;

            var listaMaterialAcao = await LitarAcaoSaidaEhRetorno(idSolicitacaoSaidaMaterial, cancellationToken);

            if (listaMaterialAcao.IsFailure)
                return listaMaterialAcao.Failure;

            var historicoProrrogacao = await ListarDatasDeProrrogacaoPorSolicitacaoDeSaidaDeMaterial(
                idSolicitacaoSaidaMaterial,
                SaidaMaterialTipoAcao.SolicitacaoProrrogacao,
                cancellationToken);

            if (historicoProrrogacao.IsFailure)
                return historicoProrrogacao.Failure;

            var historicoBaixaSemRetorno = await ListarHistoricoBaixaSemRetorno(
                    idSolicitacaoSaidaMaterial,
                    cancellationToken);

            if (historicoBaixaSemRetorno.IsFailure)
                return historicoBaixaSemRetorno.Failure;

            ProcessarStatusHistorico(saidaMaterial, listaMaterialAcao, historicoBaixaSemRetorno);

           return  new HistoricoResponseModel()
            {
                SolicitacaoMaterial = saidaMaterial.Success,
                SolicitacaoMaterialAcao = listaMaterialAcao.Success,
                HistoricoProrogacao = historicoProrrogacao.Success,
                HistoricoBaixaSemRetorno = historicoBaixaSemRetorno.Success
            };
        }

        private  void ProcessarStatusHistorico(TryException<SolicitacaoSaidaMaterialModel> saidaMaterial, TryException<IEnumerable<SolicitacaoSaidaMaterialAcaoModel>> listaMaterialAcao, TryException<IEnumerable<HistoricoProggoracaoModel>> historicoBaixaSemRetorno)
        {
            saidaMaterial.Success.ItemMaterial.ForEach(item =>
            {

                if (listaMaterialAcao.Success.Any(x => x.SolicitacaoMaterialAcaoItemId == item.IdSolicitacaoSaidaMaterialItem && x.IdSaidaMaterialTipoAcao == (int)SaidaMaterialNotaFiscalTipoAcao.RegistroSaida))
                    item.Status = StatusHistoricoMaterial.PendenteRetorno.GetDescription();

                if (listaMaterialAcao.Success.Any(x => x.SolicitacaoMaterialAcaoItemId == item.IdSolicitacaoSaidaMaterialItem && x.IdSaidaMaterialTipoAcao == (int)SaidaMaterialNotaFiscalTipoAcao.RegistroRetorno))
                    item.Status = StatusHistoricoMaterial.Concluido.GetDescription();

                if (historicoBaixaSemRetorno.Success.Any(x => x.SolicitacaoMaterialItemId == item.IdSolicitacaoSaidaMaterialItem && x.StatusId == (int)StatusCiencia.Concluido))
                    item.Status = StatusHistoricoMaterial.BaixaSemRetorno.GetDescription();

                if (saidaMaterial.Success.StatusId == (int)SolicitacaoSaidaMaterialStatus.Concluido && item.Status != StatusHistoricoMaterial.BaixaSemRetorno.GetDescription())
                    item.Status = StatusHistoricoMaterial.Concluido.GetDescription();

                if (String.IsNullOrEmpty(item.Status))
                    item.Status = StatusHistoricoMaterial.PendenteSaida.GetDescription();

            });
        }
    }
}
