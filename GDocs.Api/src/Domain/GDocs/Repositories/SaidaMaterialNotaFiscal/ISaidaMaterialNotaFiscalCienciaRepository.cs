using ICE.GDocs.Domain.Core.Repositories;
using ICE.GDocs.Infra.CrossCutting.Models;
using ICE.GDocs.Infra.CrossCutting.Models.SaidaMaterialNotaFiscal;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace ICE.GDocs.Domain.GDocs.Repositories.SaidaMaterialNotaFiscal
{
    public interface ISaidaMaterialNotaFiscalCienciaRepository : IRepository
    {
        Task<TryException<IEnumerable<ProcessoAssinaturaDocumentoModel>>> ListarCienciasPendentesDeAprovacaoPeloUsuario(
            Guid activeDirectoryId,
            CancellationToken cancellationToken
        );

        Task<TryException<SaidaMaterialNotaFiscalCienciaModel>> ObterCienciaPeloIdentificador(int idSaidaMaterialNotaFiscalCiencia, CancellationToken cancellationToken);

        Task<TryException<IEnumerable<CienciaUsuarioAprovacaoModel>>> ListarUsuarioAprovacaoPorId(int IdSolicitacaoCienciaNf, CancellationToken cancellationToken);

        Task<TryException<int>> Inserir(SaidaMaterialNotaFiscalCienciaModel saidaMaterialNotaFiscalCienciaModel, CancellationToken cancellationToken);

        Task<TryException<int?>> ObterTipoDeCienciaAtravesDoTipoDeAcao(int idTipoAcao, CancellationToken cancellationToken);

        Task<TryException<IEnumerable<CienciaUsuariosProvacao>>> ObterAprovadoresPorCiencia(int idSaidaMaterialNotaFiscalCiencia, CancellationToken cancellationToken);
        Task<TryException<int>> RegistroCiencia(SaidaMaterialNotaFiscalCienciaModel saidaMaterialNotaFiscalCienciaModel, int idCienciaAprovador, CancellationToken cancellationToken);

        Task<TryException<CienciaUsuariosProvacao>> ObterAprovadoresPorUsuarioECiencia(int idSaidaMaterialNotaFiscalCiencia, Guid usuarioId, CancellationToken cancellationToken);
        Task<TryException<int>> AtualizarCienciaStatus(SaidaMaterialNotaFiscalCienciaModel saidaMaterialNotaFiscalCienciaModel, CancellationToken cancellationToken);

    }
}
