using ICE.GDocs.Domain.Core.Repositories;
using ICE.GDocs.Infra.CrossCutting.Models;
using ICE.GDocs.Infra.CrossCutting.Models.Enums;
using ICE.GDocs.Infra.CrossCutting.Models.SaidaMaterialNotaFiscal;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace ICE.GDocs.Domain.GDocs.Repositories.SaidaMaterialNotaFiscal
{
    public interface ISaidaMaterialNotaFiscalAcaoRepository : IRepository
    {
        Task<TryException<IEnumerable<SaidaMaterialNotaFiscalTipoAcaoModel>>> ListarAcaoTipo();
        Task<TryException<int>> InserirAcao(SaidaMaterialNotaFiscalAcaoModel saidaMaterialNotaFiscalAcaoModel, CancellationToken cancellationToken);
        Task<TryException<bool>> InserirAcaoItem(SaidaMaterialNotaFiscalAcaoItemModel saidaMaterialNotaFiscalAcaoItemModel, CancellationToken cancellationToken);
        Task<TryException<IEnumerable<SaidaMaterialNotaFiscalAcaoItemModel>>> ObterItensPorIdSolicitacaoSaidaMaterialETipoAcao(int idSaidaMaterialNotaFiscal, SaidaMaterialNotaFiscalTipoAcao saidaMaterialTiposAcao, CancellationToken cancellationToken);
        Task<TryException<IEnumerable<HistoricoProrrogacaoNotaFiscalModel>>> ListarDatasDeProrrogacaoPorSolicitacaoDeSaidaDeMaterial(int idSaidaMaterialNotaFiscal, SaidaMaterialNotaFiscalTipoAcao tipoAcao, CancellationToken cancellationToken);
        Task<TryException<IEnumerable<HistoricoProrrogacaoNotaFiscalModel>>> ListarHistoricoBaixaSemRetorno(int idSaidaMaterialNotaFiscal, CancellationToken cancellationToken);
        Task<TryException<DateTime?>> ObterDataDeRetornoDaSolicitacaoDeSaidaDeMaterial(int idSaidaMaterialNotaFiscal, CancellationToken cancellationToken);
        Task<TryException<DateTime?>> ObterDataOriginalDeRetornoDaSolicitacaoDeSaidaDeMaterial(int idSaidaMaterialNotaFiscal, CancellationToken cancellationToken);
        Task<TryException<IEnumerable<SaidaMaterialNotaFiscalAcaoModel>>> LitarAcaoSaidaEhRetorno(int idSaidaMaterialNotaFiscal, CancellationToken cancellationToken);
        Task<TryException<IEnumerable<HistoricoTrocaAnexoSaidaModel>>> ListarHistoricoAnexoSaida(int idSolicitacaoSaidaMaterial, CancellationToken cancellationToken);
    }
}
