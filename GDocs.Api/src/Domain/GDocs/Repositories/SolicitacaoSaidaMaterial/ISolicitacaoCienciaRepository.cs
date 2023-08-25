using ICE.GDocs.Domain.Core.Repositories;
using ICE.GDocs.Infra.CrossCutting.Models;
using ICE.GDocs.Infra.CrossCutting.Models.SaidaMaterialNotaFiscal;
using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;

namespace ICE.GDocs.Domain.GDocs.Repositories.SolicitacaoCiencia
{
    public interface ISolicitacaoCienciaRepository : IRepository
    {
        Task<TryException<IEnumerable<ProcessoAssinaturaDocumentoModel>>> ListarCienciasPendentesDeAprovacaoPeloUsuario(
            Guid activeDirectoryId, 
            CancellationToken cancellationToken
        );

        Task<TryException<SolicitacaoCienciaModel>> ObterCienciaPeloIdentificador(int idSolicitacaoCiencia, CancellationToken cancellationToken);

        Task<TryException<int>> Inserir(SolicitacaoCienciaModel solicitacaoCiencia, CancellationToken cancellationToken);

        Task<TryException<int?>> ObterTipoDeCienciaAtravesDoTipoDeAcao(int idTipoAcao, CancellationToken cancellationToken);

        Task<TryException<IEnumerable<CienciaUsuariosProvacao>>> ObterAprovadoresPorCiencia(int idSolicitacaoCiencia, CancellationToken cancellationToken);
        Task<TryException<int>> RegistroCiencia(SolicitacaoCienciaModel solicitacaoCiencia, int idCienciaAprovador, CancellationToken cancellationToken);

        Task<TryException<CienciaUsuariosProvacao>> ObterAprovadoresPorUsuarioECiencia(int idSolicitacaoCiencia, Guid usuarioId, CancellationToken cancellationToken);
        Task<TryException<int>> AtualizarCienciaStatus(SolicitacaoCienciaModel solicitacaoCiencia, CancellationToken cancellationToken);

        Task<TryException<IEnumerable<CienciaUsuarioAprovacaoModel>>> ListarObsUsuarioPorCiencia(int IdSolicitacaoCiencia, CancellationToken cancellationToken);

        Task<TryException<SoclicitacaoCienciaAprovadoresModel>> ObterAprovadoresCienciaCancelamento(int solicitacaoSaidaMaterialId, CancellationToken cancellationToken);
    }
}
