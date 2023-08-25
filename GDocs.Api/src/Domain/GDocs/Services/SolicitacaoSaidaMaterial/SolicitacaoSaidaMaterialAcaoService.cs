using ICE.GDocs.Domain.Core.Services;
using ICE.GDocs.Domain.Core.Uow;
using ICE.GDocs.Domain.GDocs.Repositories.SolicitacaoCiencia;
using ICE.GDocs.Domain.GDocs.Repositories.SolicitacaoSaidaMaterial;
using ICE.GDocs.Infra.CrossCutting.Models;
using ICE.GDocs.Infra.CrossCutting.Models.Enums;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace ICE.GDocs.Domain.GDocs.Services.SolicitacaoSaidaMaterial
{
    internal class SolicitacaoSaidaMaterialAcaoService : DomainService, ISolicitacaoSaidaMaterialAcaoService
    {
        private readonly ISolicitacaoSaidaMaterialRepository _solicitacaoSaidaMaterialRepository;
        private readonly ISolicitacaoSaidaMaterialItemRepository _solicitacaoSaidaMaterialItemRepository;
        private readonly ISolicitacaoSaidaMaterialAcaoRepository _solicitacaoSaidaMaterialAcaoRepository;
        private readonly ISolicitacaoCienciaRepository _solicitacaoCienciaRepository;

        public SolicitacaoSaidaMaterialAcaoService(
            IUnitOfWork unitOfWork,
            ISolicitacaoSaidaMaterialAcaoRepository solicitacaoSaidaMaterialAcaoRepository,
            ISolicitacaoCienciaRepository solicitacaoCienciaRepository,
            ISolicitacaoSaidaMaterialRepository solicitacaoSaidaMaterialRepository,
            ISolicitacaoSaidaMaterialItemRepository solicitacaoSaidaMaterialItemRepository) : base(unitOfWork)
        {
            _solicitacaoSaidaMaterialAcaoRepository = solicitacaoSaidaMaterialAcaoRepository;
            _solicitacaoCienciaRepository = solicitacaoCienciaRepository;
            _solicitacaoSaidaMaterialRepository = solicitacaoSaidaMaterialRepository;
            _solicitacaoSaidaMaterialItemRepository = solicitacaoSaidaMaterialItemRepository;
        }

        public async Task<TryException<int>> InserirAcao(Guid usuarioLogado, SolicitacaoSaidaMaterialAcaoModel solicitacaoSaidaMaterialAcaoModel, CancellationToken cancellationToken)
        {
            using (var transaction = _unitOfWork.BeginTransaction())
            {
                var result = await _solicitacaoSaidaMaterialAcaoRepository.InserirAcao(solicitacaoSaidaMaterialAcaoModel, cancellationToken);

                solicitacaoSaidaMaterialAcaoModel.DefinirId(result.Success);

                foreach (var acaoItem in solicitacaoSaidaMaterialAcaoModel?.AcaoItems)
                {
                    acaoItem.IdSolicitacaoSaidaMaterialAcao = result.Success;
                    await _solicitacaoSaidaMaterialAcaoRepository.InserirAcaoItem(acaoItem, cancellationToken);
                }

                var tipoCiencia = await _solicitacaoCienciaRepository.ObterTipoDeCienciaAtravesDoTipoDeAcao(solicitacaoSaidaMaterialAcaoModel.IdSaidaMaterialTipoAcao, cancellationToken);

                if (tipoCiencia.IsFailure)
                    return tipoCiencia.Failure;

                if (tipoCiencia.Success.HasValue)
                {
                    var ciencia = await GerarSolicitacaoDeCiencia(usuarioLogado, solicitacaoSaidaMaterialAcaoModel, tipoCiencia.Success.Value, cancellationToken);

                    if (ciencia.IsFailure)
                        return ciencia.Failure;
                }

                await this.AtualizarStatusSilicitacaoSaidaMaterial(solicitacaoSaidaMaterialAcaoModel, cancellationToken);

                transaction.Commit();

                return result;
            }
        }

        public async Task<TryException<IEnumerable<SolicitacaoSaidaMaterialAcaoTipoModel>>> ListarAcaoTipo()
            => await _solicitacaoSaidaMaterialAcaoRepository.ListarAcaoTipo();

        public async Task<TryException<IEnumerable<HistoricoProggoracaoModel>>> ListarDatasDeProrrogacaoPorSolicitacaoDeSaidaDeMaterial(int idSolicitacaoSaidaMaterial, SaidaMaterialTipoAcao tipoAcao, CancellationToken cancellationToken)
        {
            var historicoPrrogacao = await _solicitacaoSaidaMaterialAcaoRepository.ListarDatasDeProrrogacaoPorSolicitacaoDeSaidaDeMaterial(idSolicitacaoSaidaMaterial,tipoAcao, cancellationToken);

            if (historicoPrrogacao.IsFailure)
                return historicoPrrogacao.Failure;

            return historicoPrrogacao;
        }

        private async Task<TryException<int>> GerarSolicitacaoDeCiencia(Guid usuarioLogado, SolicitacaoSaidaMaterialAcaoModel solicitacaoSaidaMaterialAcaoModel, int idTipoCiencia, CancellationToken cancellationToken)
        {
            var ciencia = new SolicitacaoCienciaModel()
            {
                IdSolicitacaoSaidaMaterial = solicitacaoSaidaMaterialAcaoModel.IdSolicitacaoSaidaMaterial,
                IdSolicitacaoSaidaMaterialAcao = solicitacaoSaidaMaterialAcaoModel.Id,
                IdStatusCiencia = StatusCiencia.Pendente.ToInt32(),
                IdTipoCiencia = idTipoCiencia,
                DataProrrogacao = idTipoCiencia == TipoCiencia.Prorrogação.ToInt32() ? solicitacaoSaidaMaterialAcaoModel.DataAcao : (DateTime?)null,
                IdUsuario = usuarioLogado,
                Observacao = solicitacaoSaidaMaterialAcaoModel?.Observacao,
                FlgAtivo = true
            };

            foreach(var reg in solicitacaoSaidaMaterialAcaoModel.AcaoItems)
            {
                ciencia.Itens.Add(new SolicitacaoCienciaItemModel { IdSolicitacaoSaidaMaterialItem = reg.IdSolicitacaoSaidaMaterialItem });
            }

            return await _solicitacaoCienciaRepository.Inserir(ciencia, cancellationToken);
        }

        private async Task<TryException<Return>> AtualizarStatusSilicitacaoSaidaMaterial(SolicitacaoSaidaMaterialAcaoModel solicitacaoSaidaMaterialAcaoModel, CancellationToken cancellationToken)
        {
            var solicitacaoSaidaMaterial = await _solicitacaoSaidaMaterialRepository.ObterPorId(solicitacaoSaidaMaterialAcaoModel.IdSolicitacaoSaidaMaterial, cancellationToken);

            if (solicitacaoSaidaMaterial.IsFailure)
                return solicitacaoSaidaMaterial.Failure;

            switch (solicitacaoSaidaMaterialAcaoModel.IdSaidaMaterialTipoAcao)
            {
                case (int)SaidaMaterialTipoAcao.RegistroSaida:
                    await AtualizarStatusRegistroSaida(solicitacaoSaidaMaterial.Success, cancellationToken);
                    break;

                case (int)SaidaMaterialTipoAcao.RegistroRetorno:
                    await AtualizarStatusRegistroRetorno(solicitacaoSaidaMaterialAcaoModel, cancellationToken);
                    break;

                case (int)SaidaMaterialTipoAcao.SolicitacaoProrrogacao:
                case (int)SaidaMaterialTipoAcao.SolicitacaoBaixaSemRetorno:
                case (int)SaidaMaterialTipoAcao.SolicitacaoCancelamento:
                    await AtualizarStatusRegistroCiencia(solicitacaoSaidaMaterialAcaoModel, cancellationToken);
                    break;

            }

            return Return.Empty;
        }

        private async Task<TryException<Return>> AtualizarStatusRegistroSaida(SolicitacaoSaidaMaterialModel solicitacaoSaidaMaterialModel, CancellationToken cancellationToken)
        {
            if (solicitacaoSaidaMaterialModel.FlgRetorno)
                await _solicitacaoSaidaMaterialRepository.AtualizarStatus(solicitacaoSaidaMaterialModel.Id, SolicitacaoSaidaMaterialStatus.EmAberto, cancellationToken);
            else
                await _solicitacaoSaidaMaterialRepository.AtualizarStatus(solicitacaoSaidaMaterialModel.Id, SolicitacaoSaidaMaterialStatus.Concluido, cancellationToken);

            return Return.Empty;
        }

        private async Task<TryException<Return>> AtualizarStatusRegistroCiencia(
        SolicitacaoSaidaMaterialAcaoModel solicitacaoSaidaMaterialAcaoModel,
        CancellationToken cancellationToken)
        {
            var tipoCiencia = await _solicitacaoCienciaRepository.ObterTipoDeCienciaAtravesDoTipoDeAcao(solicitacaoSaidaMaterialAcaoModel.IdSaidaMaterialTipoAcao, cancellationToken);

            if (tipoCiencia.IsFailure)
                return tipoCiencia.Failure;

            if (tipoCiencia.Success.HasValue)
            {
                await _solicitacaoSaidaMaterialRepository.AtualizarStatus(solicitacaoSaidaMaterialAcaoModel.IdSolicitacaoSaidaMaterial, SolicitacaoSaidaMaterialStatus.AprovacaoCiencia, cancellationToken);
                return Return.Empty;
            }

            return Return.Empty;

        }


        private async Task<TryException<Return>> AtualizarStatusRegistroRetorno(
                 SolicitacaoSaidaMaterialAcaoModel solicitacaoSaidaMaterialAcaoModel,
                 CancellationToken cancellationToken)
        {
            var solicitacaoSaidaMaterialItemRepository = await _solicitacaoSaidaMaterialItemRepository.ObterPorIdSolicitacaoSaidaMaterial(solicitacaoSaidaMaterialAcaoModel.IdSolicitacaoSaidaMaterial, cancellationToken);
            if (solicitacaoSaidaMaterialItemRepository.IsFailure)
                return solicitacaoSaidaMaterialItemRepository.Failure;
           
            if (solicitacaoSaidaMaterialAcaoModel.AcaoItems?.Count() == solicitacaoSaidaMaterialItemRepository.Success.Count())
            {
                await _solicitacaoSaidaMaterialRepository.AtualizarStatus(solicitacaoSaidaMaterialAcaoModel.IdSolicitacaoSaidaMaterial, SolicitacaoSaidaMaterialStatus.Concluido, cancellationToken);
            }
            else
            {
                var totalItemMaterialBaixado = await _solicitacaoSaidaMaterialItemRepository.ObterQuantidadeItemComBaixa(solicitacaoSaidaMaterialAcaoModel.IdSolicitacaoSaidaMaterial, cancellationToken);
                if (totalItemMaterialBaixado.IsFailure) return totalItemMaterialBaixado.Failure;

                if (solicitacaoSaidaMaterialItemRepository.Success.Count() == totalItemMaterialBaixado.Success)
                    await _solicitacaoSaidaMaterialRepository.AtualizarStatus(solicitacaoSaidaMaterialAcaoModel.IdSolicitacaoSaidaMaterial, SolicitacaoSaidaMaterialStatus.Concluido, cancellationToken);
                else
                    await _solicitacaoSaidaMaterialRepository.AtualizarStatus(solicitacaoSaidaMaterialAcaoModel.IdSolicitacaoSaidaMaterial, SolicitacaoSaidaMaterialStatus.EmAberto, cancellationToken, true);
            }
            return Return.Empty;
        }

        public Task<TryException<IEnumerable<SolicitacaoSaidaMaterialAcaoModel>>> LitarAcaoSaidaEhRetorno(int idSolicitacaoSaidaMaterial, CancellationToken cancellationToken)
             => _solicitacaoSaidaMaterialAcaoRepository.LitarAcaoSaidaEhRetorno(idSolicitacaoSaidaMaterial,cancellationToken);

        public async Task<TryException<IEnumerable<HistoricoProggoracaoModel>>> ListarHistoricoBaixaSemRetorno(int idSolicitacaoSaidaMaterial, CancellationToken cancellationToken)
        {
            var historicoBaixaSemRetorno = await _solicitacaoSaidaMaterialAcaoRepository.ListarHistoricoBaixaSemRetorno(idSolicitacaoSaidaMaterial, cancellationToken);

            if (historicoBaixaSemRetorno.IsFailure)
                return historicoBaixaSemRetorno.Failure;

            return historicoBaixaSemRetorno;
        }
    }
}