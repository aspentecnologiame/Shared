using ICE.GDocs.Domain.ExternalServices;
using Microsoft.Extensions.Configuration;
using System;
using System.Collections.Generic;
using System.Text;

namespace ICE.GDocs.Domain.Services
{
    public class AssinaturaAgregationService : IAssinaturaAgregationService
    {
        private readonly IDocToolsExternalService _docToolsExternalService;
        private readonly ILogService _logService;
        private readonly IConfiguration _configuration;
        private readonly IProcessoAssinaturaDocumentoService _processoAssinaturaDocumentoService;
        private readonly IGdocsWorkerExternalService _gdocsWorkerExternalService;


        public IDocToolsExternalService DocToolsExternalService => _docToolsExternalService;
        public ILogService LogService => _logService;
        public IConfiguration Configuration => _configuration;
        public IProcessoAssinaturaDocumentoService ProcessoAssinaturaDocumentoService => _processoAssinaturaDocumentoService;
        public IGdocsWorkerExternalService GdocsWorkerExternalService => _gdocsWorkerExternalService;

        public AssinaturaAgregationService
        (
            IDocToolsExternalService docToolsExternalService,
            ILogService logService,
            IConfiguration configuration,
            IProcessoAssinaturaDocumentoService processoAssinaturaDocumentoService,
            IGdocsWorkerExternalService gdocsWorkerExternalService
        )
        {
            _docToolsExternalService = docToolsExternalService;
            _logService = logService;
            _configuration = configuration;
            _processoAssinaturaDocumentoService = processoAssinaturaDocumentoService;
            _gdocsWorkerExternalService = gdocsWorkerExternalService;
        }
    }
}
