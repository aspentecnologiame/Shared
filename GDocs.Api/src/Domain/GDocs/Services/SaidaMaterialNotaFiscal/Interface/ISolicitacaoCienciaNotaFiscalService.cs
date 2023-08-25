using ICE.GDocs.Infra.CrossCutting.Models;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;
using System.Threading;
using ICE.GDocs.Domain.Core.Services;
using ICE.GDocs.Infra.CrossCutting.Models.SaidaMaterialNotaFiscal;

namespace ICE.GDocs.Domain.GDocs.Services.SaidaMaterialNotaFiscal.Interface
{
    public interface ISolicitacaoCienciaNotaFiscalService : IDomainService
    {
        Task<TryException<IEnumerable<ProcessoAssinaturaDocumentoModel>>> ListarCienciasPendentesParaAprovacaoPeloUsuario(Guid activeDirectoryId, CancellationToken cancellationToken);
        Task<TryException<SaidaMaterialNotaFiscalCienciaModel>> ObterCienciaNfPorId(int IdSolicitacaoCienciaNf, CancellationToken cancellationToken);
        Task<TryException<Return>> RegistroCienciaPorUsuario(SaidaMaterialNotaFiscalCienciaModel solicitacaoCienciaNfModel, Guid usuarioLogadoId, CancellationToken cancellationToken);

        Task<TryException<IEnumerable<CienciaUsuarioAprovacaoModel>>> ListarUsuarioAprovacao(int IdSolicitacaoCienciaNf, CancellationToken cancellationToken);
    }
}
