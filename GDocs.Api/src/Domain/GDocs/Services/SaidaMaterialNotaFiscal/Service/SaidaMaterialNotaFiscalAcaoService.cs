using ICE.GDocs.Domain.Core.Services;
using ICE.GDocs.Domain.Core.Uow;
using ICE.GDocs.Domain.GDocs.Repositories.SaidaMaterialNotaFiscal;
using ICE.GDocs.Domain.GDocs.Services.SaidaMaterialNotaFiscal.Interface;
using ICE.GDocs.Infra.CrossCutting.Models.Enums;
using ICE.GDocs.Infra.CrossCutting.Models.SaidaMaterialNotaFiscal;
using ICE.GDocs.Infra.CrossCutting.Models.SaidaMaterialNotaFiscal.Enums;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

namespace ICE.GDocs.Domain.GDocs.Services.SolicitacaoSaidaMaterial
{
    internal class SaidaMaterialNotaFiscalAcaoService : DomainService, ISaidaMaterialNotaFiscalAcaoService
    {
        private readonly ISaidaMaterialNotaFiscalRepository _saidaMaterialNotaFiscalRepository;
        private readonly ISaidaMaterialNotaFiscalAcaoRepository _saidaMaterialNotaFiscalAcaoRepository;
        private readonly ISaidaMaterialNotaFiscalCienciaRepository _saidaMaterialNotaFiscalCienciaRepository;

        public SaidaMaterialNotaFiscalAcaoService(
            IUnitOfWork unitOfWork,
            ISaidaMaterialNotaFiscalAcaoRepository saidaMaterialNotaFiscalAcaoRepository,
            ISaidaMaterialNotaFiscalCienciaRepository saidaMaterialNotaFiscalCienciaRepository,
            ISaidaMaterialNotaFiscalRepository saidaMaterialNotaFiscalRepository) : base(unitOfWork)
        {
            _saidaMaterialNotaFiscalAcaoRepository = saidaMaterialNotaFiscalAcaoRepository;
            _saidaMaterialNotaFiscalCienciaRepository = saidaMaterialNotaFiscalCienciaRepository;
            _saidaMaterialNotaFiscalRepository = saidaMaterialNotaFiscalRepository;
        }

        public async Task<TryException<int>> InserirAcao(Guid usuarioLogado, SaidaMaterialNotaFiscalAcaoModel saidaMaterialNotaFiscalAcaoModel, CancellationToken cancellationToken)
        {
            using (var transaction = _unitOfWork.BeginTransaction())
            {
                var result = await _saidaMaterialNotaFiscalAcaoRepository.InserirAcao(saidaMaterialNotaFiscalAcaoModel, cancellationToken);

                saidaMaterialNotaFiscalAcaoModel.DefinirId(result.Success);

                foreach (var acaoItem in saidaMaterialNotaFiscalAcaoModel?.AcaoItems)
                {
                    acaoItem.IdSaidaMaterialNotaFiscalAcao = result.Success;
                    await _saidaMaterialNotaFiscalAcaoRepository.InserirAcaoItem(acaoItem, cancellationToken);
                }

                var tipoCiencia = await _saidaMaterialNotaFiscalCienciaRepository.ObterTipoDeCienciaAtravesDoTipoDeAcao(saidaMaterialNotaFiscalAcaoModel.IdSaidaMaterialNotaFiscalTipoAcao, cancellationToken);

                if (tipoCiencia.IsFailure)
                    return tipoCiencia.Failure;

                if (tipoCiencia.Success.HasValue)
                {
                    var ciencia = await GerarSolicitacaoDeCiencia(usuarioLogado, saidaMaterialNotaFiscalAcaoModel, tipoCiencia.Success.Value, cancellationToken);

                    if (ciencia.IsFailure)
                        return ciencia.Failure;
                }

                await this.AtualizarStatusSilicitacaoSaidaMaterial(saidaMaterialNotaFiscalAcaoModel, cancellationToken);

                transaction.Commit();

                return result;
            }
        }

        public async Task<TryException<IEnumerable<SaidaMaterialNotaFiscalTipoAcaoModel>>> ListarAcaoTipo()
            => await _saidaMaterialNotaFiscalAcaoRepository.ListarAcaoTipo();

        public async Task<TryException<IEnumerable<HistoricoProrrogacaoNotaFiscalModel>>> ListarDatasDeProrrogacaoPorSolicitacaoDeSaidaDeMaterial(int idSaidaMaterialNotaFiscal, SaidaMaterialNotaFiscalTipoAcao tipoAcao, CancellationToken cancellationToken)
        {
            var historicoPrrogacao = await _saidaMaterialNotaFiscalAcaoRepository.ListarDatasDeProrrogacaoPorSolicitacaoDeSaidaDeMaterial(idSaidaMaterialNotaFiscal, tipoAcao, cancellationToken);

            if (historicoPrrogacao.IsFailure)
                return historicoPrrogacao.Failure;

            return historicoPrrogacao;
        }

        private async Task<TryException<int>> GerarSolicitacaoDeCiencia(Guid usuarioLogado, SaidaMaterialNotaFiscalAcaoModel saidaMaterialNotaFiscalAcaoModel, int idTipoCiencia, CancellationToken cancellationToken)
        {
            var ciencia = new SaidaMaterialNotaFiscalCienciaModel()
            {
                IdSaidaMaterialNotaFiscal = saidaMaterialNotaFiscalAcaoModel.IdSaidaMaterialNotaFiscal,
                IdSaidaMaterialNotaFiscalAcao = saidaMaterialNotaFiscalAcaoModel.Id,
                IdStatusCiencia = StatusCiencia.Pendente.ToInt32(),
                IdTipoCiencia = idTipoCiencia,
                DataProrrogacaoNF = idTipoCiencia == TipoCiencia.Prorrogação.ToInt32() ? saidaMaterialNotaFiscalAcaoModel.DataAcao : (DateTime?)null,
                IdUsuario = usuarioLogado,
                Observacao = saidaMaterialNotaFiscalAcaoModel?.Observacao,
                FlgAtivo = true
            };

            foreach (var reg in saidaMaterialNotaFiscalAcaoModel.AcaoItems)
            {
                ciencia.Itens.Add(new SaidaMaterialNotaFiscalCienciaItemModel { IdSaidaMaterialNotaFiscalItem = reg.IdSaidaMaterialNotaFiscalItem });
            }

            return await _saidaMaterialNotaFiscalCienciaRepository.Inserir(ciencia, cancellationToken);
        }

        private async Task<TryException<Return>> AtualizarStatusSilicitacaoSaidaMaterial(SaidaMaterialNotaFiscalAcaoModel saidaMaterialNotaFiscalAcaoModel, CancellationToken cancellationToken)
        {
            var saidaMaterialNotaFiscal = await _saidaMaterialNotaFiscalRepository.ObterMaterialNotaFiscalPorId(saidaMaterialNotaFiscalAcaoModel.IdSaidaMaterialNotaFiscal, cancellationToken);

            if (saidaMaterialNotaFiscal.IsFailure)
                return saidaMaterialNotaFiscal.Failure;

            switch (saidaMaterialNotaFiscalAcaoModel.IdSaidaMaterialNotaFiscalTipoAcao)
            {
                case (int)SaidaMaterialNotaFiscalTipoAcao.RegistroSaida:
                    await AtualizarStatusRegistroSaida(saidaMaterialNotaFiscal.Success, cancellationToken);
                    break;

                case (int)SaidaMaterialNotaFiscalTipoAcao.RegistroRetorno:
                    await _saidaMaterialNotaFiscalRepository.AtualizarStatus(saidaMaterialNotaFiscal.Success, SaidaMaterialNotaFiscalStatus.PendenteNFRetorno, cancellationToken);
                    break;

                case (int)SaidaMaterialNotaFiscalTipoAcao.SolicitacaoProrrogacao:
                    await AtualizarStatusRegistroCiencia(saidaMaterialNotaFiscalAcaoModel, saidaMaterialNotaFiscal.Success, cancellationToken);
                    break;

                case (int)SaidaMaterialNotaFiscalTipoAcao.SolicitacaoBaixaSemRetorno:
                    await AtualizarStatusRegistroCiencia(saidaMaterialNotaFiscalAcaoModel, saidaMaterialNotaFiscal.Success, cancellationToken);
                    break;

            }

            return Return.Empty;
        }

        private async Task<TryException<Return>> AtualizarStatusRegistroSaida(SaidaMaterialNotaFiscalModel saidaMaterialNotaFiscalModel, CancellationToken cancellationToken)
        {
            if (saidaMaterialNotaFiscalModel.FlgRetorno)
                await _saidaMaterialNotaFiscalRepository.AtualizarStatus(saidaMaterialNotaFiscalModel, SaidaMaterialNotaFiscalStatus.EmAberto, cancellationToken);
            else
                await _saidaMaterialNotaFiscalRepository.AtualizarStatus(saidaMaterialNotaFiscalModel, SaidaMaterialNotaFiscalStatus.Concluida, cancellationToken);

            return Return.Empty;
        }

        private async Task<TryException<Return>> AtualizarStatusRegistroCiencia(
        SaidaMaterialNotaFiscalAcaoModel saidaMaterialNotaFiscalAcaoModel,
        SaidaMaterialNotaFiscalModel saidaMaterialNotaFiscalModel,
        CancellationToken cancellationToken)
        {
            var tipoCiencia = await _saidaMaterialNotaFiscalCienciaRepository.ObterTipoDeCienciaAtravesDoTipoDeAcao(saidaMaterialNotaFiscalAcaoModel.IdSaidaMaterialNotaFiscalTipoAcao, cancellationToken);

            if (tipoCiencia.IsFailure)
                return tipoCiencia.Failure;

            if (tipoCiencia.Success.HasValue)
            {
                await _saidaMaterialNotaFiscalRepository.AtualizarStatus(saidaMaterialNotaFiscalModel, SaidaMaterialNotaFiscalStatus.EmAprovaçãoDeciencia, cancellationToken);
                return Return.Empty;
            }

            return Return.Empty;

        }


        public async Task<TryException<IEnumerable<SaidaMaterialNotaFiscalAcaoModel>>> LitarAcaoSaidaEhRetorno(int idSaidaMaterialNotaFiscal, CancellationToken cancellationToken)
             => await _saidaMaterialNotaFiscalAcaoRepository.LitarAcaoSaidaEhRetorno(idSaidaMaterialNotaFiscal, cancellationToken);

        public async Task<TryException<IEnumerable<HistoricoProrrogacaoNotaFiscalModel>>> ListarHistoricoBaixaSemRetorno(int idSolicitacaoSaidaMaterial, CancellationToken cancellationToken)
        {
            var historicoBaixaSemRetorno = await _saidaMaterialNotaFiscalAcaoRepository.ListarHistoricoBaixaSemRetorno(idSolicitacaoSaidaMaterial, cancellationToken);

            if (historicoBaixaSemRetorno.IsFailure)
                return historicoBaixaSemRetorno.Failure;

            return historicoBaixaSemRetorno;
        }

        public async Task<TryException<IEnumerable<HistoricoTrocaAnexoSaidaModel>>> ListarHistoricoAnexoSaida(int idSolicitacaoSaidaMaterial, CancellationToken cancellationToken) =>
            await _saidaMaterialNotaFiscalAcaoRepository.ListarHistoricoAnexoSaida(idSolicitacaoSaidaMaterial, cancellationToken);
             
      
    }
}