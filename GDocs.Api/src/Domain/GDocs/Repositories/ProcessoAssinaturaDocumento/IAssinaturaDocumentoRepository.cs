using ICE.GDocs.Domain.Core.Repositories;
using ICE.GDocs.Infra.CrossCutting.Models;
using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;

namespace ICE.GDocs.Domain.Repositories.ProcessoAssinaturaDocumento
{
    public interface IAssinaturaDocumentoRepository : IRepository
    {
        Task<TryException<IEnumerable<ProcessoAssinaturaDocumentoModel>>> ListarDocumentosPendentesDeAssinaturaOuJaAssinadoPeloUsuario(Guid activeDirectoryId, CancellationToken cancellationToken);

        Task<TryException<IEnumerable<ProcessoAssinaturaDocumentoModel>>> ListarAssinadosRecentemente(Guid activeDirectoryId, List<ProcessoAssinaturaDocumentoModel> processoAssinaturaDocumentoModel, CancellationToken cancellationToken);
    }
}
