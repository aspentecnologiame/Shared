using System;
using System.Threading;
using System.Threading.Tasks;

namespace ICE.GDocs.Domain.ExternalServices
{
    public interface IGdocsWorkerExternalService
    {
        Task<TryException<Return>> EmitirAlteracaoStatusDocumento(int processoAssinaturaDocumentoId, Guid correlationId = default, CancellationToken cancellationToken = default);
    }
}
