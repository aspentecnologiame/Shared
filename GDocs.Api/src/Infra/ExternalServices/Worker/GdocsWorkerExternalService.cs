using ICE.GDocs.Domain.ExternalServices;
using ICE.GDocs.Worker.Lib;
using System;
using System.Threading;
using System.Threading.Tasks;

namespace Infra.ExternalServices.GdocsWorker
{
    internal partial class GdocsWorkerExternalService : IGdocsWorkerExternalService
    {
        private readonly IGDocsMessageBusClient _gDocsMessageBusClient;

        public GdocsWorkerExternalService(IGDocsMessageBusClient gDocsMessageBusClient)
        {
            _gDocsMessageBusClient = gDocsMessageBusClient;
        }

        public async Task<TryException<Return>> EmitirAlteracaoStatusDocumento(int processoAssinaturaDocumentoId, Guid correlationId = default, CancellationToken cancellationToken = default)
        {
            var emitirAlteracaoStatusDocumento = await _gDocsMessageBusClient.EmitirAlteracaoStatusDocumento(processoAssinaturaDocumentoId, correlationId, cancellationToken);
            if (emitirAlteracaoStatusDocumento.IsFailure)
                return emitirAlteracaoStatusDocumento.Failure;

            return Return.Empty;
        }
    }
}
