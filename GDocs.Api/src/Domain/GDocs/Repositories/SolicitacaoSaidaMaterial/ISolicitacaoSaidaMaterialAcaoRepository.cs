using ICE.GDocs.Domain.Core.Repositories;
using ICE.GDocs.Infra.CrossCutting.Models;
using ICE.GDocs.Infra.CrossCutting.Models.Enums;
using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;

namespace ICE.GDocs.Domain.GDocs.Repositories.SolicitacaoSaidaMaterial
{
    public interface ISolicitacaoSaidaMaterialAcaoRepository : IRepository
    {
        Task<TryException<IEnumerable<SolicitacaoSaidaMaterialAcaoTipoModel>>> ListarAcaoTipo();
        Task<TryException<int>> InserirAcao(SolicitacaoSaidaMaterialAcaoModel solicitacaoSaidaMaterialAcaoModel, CancellationToken cancellationToken);
        Task<TryException<bool>> InserirAcaoItem(SolicitacaoSaidaMaterialAcaoItemModel solicitacaoSaidaMaterialAcaoItemModel, CancellationToken cancellationToken);
        Task<TryException<IEnumerable<SolicitacaoSaidaMaterialAcaoItemModel>>> ObterItensPorIdSolicitacaoSaidaMaterialETipoAcao(int idSolicitacaoSaidaMaterial, SaidaMaterialTipoAcao saidaMaterialTiposAcao, CancellationToken cancellationToken);
        Task<TryException<IEnumerable<HistoricoProggoracaoModel>>> ListarDatasDeProrrogacaoPorSolicitacaoDeSaidaDeMaterial(int idSolicitacaoSaidaMaterial, SaidaMaterialTipoAcao tipoAcao, CancellationToken cancellationToken);
        Task<TryException<IEnumerable<HistoricoProggoracaoModel>>> ListarHistoricoBaixaSemRetorno(int idSolicitacaoSaidaMaterial, CancellationToken cancellationToken);
        Task<TryException<DateTime?>> ObterDataDeRetornoDaSolicitacaoDeSaidaDeMaterial(int idSolicitacaoSaidaMaterial, CancellationToken cancellationToken);
        Task<TryException<DateTime?>> ObterDataOriginalDeRetornoDaSolicitacaoDeSaidaDeMaterial(int idSolicitacaoSaidaMaterial, CancellationToken cancellationToken);
        Task<TryException<IEnumerable<SolicitacaoSaidaMaterialAcaoModel>>> LitarAcaoSaidaEhRetorno(int idSolicitacaoSaidaMaterial, CancellationToken cancellationToken);
    }
}
