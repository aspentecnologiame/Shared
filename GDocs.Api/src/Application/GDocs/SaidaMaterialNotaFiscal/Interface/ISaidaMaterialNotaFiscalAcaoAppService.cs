using ICE.GDocs.Application.Core.Services;
using ICE.GDocs.Infra.CrossCutting.Models.Enums;
using ICE.GDocs.Infra.CrossCutting.Models.SaidaMaterialNotaFiscal;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace ICE.GDocs.Application.GDocs.SaidaMaterialNotaFiscal.Interface
{
    public interface ISaidaMaterialNotaFiscalAcaoAppService : IApplicationService
    {
        Task<TryException<int>> InserirAcao(Guid usuarioLogado, SaidaMaterialNotaFiscalAcaoModel saidaMaterialNotaFiscalAcaoModel, CancellationToken cancellationToken);
        Task<TryException<IEnumerable<SaidaMaterialNotaFiscalTipoAcaoModel>>> ListarAcaoTipo();
        Task<TryException<IEnumerable<SaidaMaterialNotaFiscalAcaoModel>>> LitarAcaoSaidaEhRetorno(int idSaidaMaterialNotaFiscal, CancellationToken cancellationToken);
        Task<TryException<IEnumerable<HistoricoProrrogacaoNotaFiscalModel>>> ListarDatasDeProrrogacaoPorSolicitacaoDeSaidaDeMaterial(int idSaidaMaterialNotaFiscal, SaidaMaterialNotaFiscalTipoAcao tipoAcao, CancellationToken cancellationToken);
        Task<TryException<IEnumerable<HistoricoProrrogacaoNotaFiscalModel>>> ListarHistoricoBaixaSemRetorno(int idSolicitacaoSaidaMaterial, CancellationToken cancellationToken);
        Task<TryException<IEnumerable<HistoricoTrocaAnexoSaidaModel>>> ListarHistoricoAnexoSaida(int idSolicitacaoSaidaMaterial, CancellationToken cancellationToken);

    }
}
