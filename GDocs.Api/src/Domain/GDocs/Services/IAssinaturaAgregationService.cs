using ICE.GDocs.Domain.Core.Services;
using ICE.GDocs.Domain.ExternalServices;
using Microsoft.Extensions.Configuration;
using System;
using System.Collections.Generic;
using System.Text;

namespace ICE.GDocs.Domain.Services
{
    public interface IAssinaturaAgregationService : IDomainService
    {
        IDocToolsExternalService DocToolsExternalService { get; }
        ILogService LogService { get; }
        IConfiguration Configuration { get; }
        IProcessoAssinaturaDocumentoService ProcessoAssinaturaDocumentoService { get; }
        IGdocsWorkerExternalService GdocsWorkerExternalService { get; }
    }
}
