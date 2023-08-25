using ICE.GDocs.Domain.ExternalServices;
using ICE.GDocs.Domain.GDocs.Repositories.SolicitacaoSaidaMaterial;
using ICE.GDocs.Domain.Repositories;
using ICE.GDocs.Domain.Repositories.ProcessoAssinaturaDocumento;
using ICE.GDocs.Domain.Services;

namespace ICE.GDocs.Domain.GDocs.Services.SolicitacaoSaidaMaterial
{
    internal class SolicitacaoSaidaMaterialAgregationService : ISolicitacaoSaidaMaterialAgregationService
    {
        private readonly ISolicitacaoSaidaMaterialItemRepository _solicitacaoSaidaMaterialItemRepository;
        private readonly IConfiguracaoRepository _configuracaoRepository;
        private readonly IProcessoAssinaturaDocumentoOrigemRepository _processoAssinaturaDocumentoOrigemRepository;
        private readonly ITemplateProcessoAssinaturaDocumentoRepository _templateProcessoAssinaturaDocumentoRepository;
        private readonly IProcessoAssinaturaDocumentoService _processoAssinaturaDocumentoService;
        private readonly IGdocsWorkerExternalService _gdocsWorkerExternalService;
        private readonly ILogService _logService;

        public ISolicitacaoSaidaMaterialItemRepository SolicitacaoSaidaMaterialItemRepository => _solicitacaoSaidaMaterialItemRepository;
        public IConfiguracaoRepository ConfiguracaoRepository => _configuracaoRepository;
        public IProcessoAssinaturaDocumentoOrigemRepository ProcessoAssinaturaDocumentoOrigemRepository => _processoAssinaturaDocumentoOrigemRepository;
        public ITemplateProcessoAssinaturaDocumentoRepository TemplateProcessoAssinaturaDocumentoRepository => _templateProcessoAssinaturaDocumentoRepository;
        public IProcessoAssinaturaDocumentoService ProcessoAssinaturaDocumentoService => _processoAssinaturaDocumentoService;
        public IGdocsWorkerExternalService GdocsWorkerExternalService => _gdocsWorkerExternalService;
        public ILogService LogService => _logService;

        public SolicitacaoSaidaMaterialAgregationService(
            ISolicitacaoSaidaMaterialItemRepository solicitacaoSaidaMaterialItemRepository,
            IConfiguracaoRepository configuracaoRepository,
            IProcessoAssinaturaDocumentoOrigemRepository processoAssinaturaDocumentoOrigemRepository,
            ITemplateProcessoAssinaturaDocumentoRepository templateProcessoAssinaturaDocumentoRepository,
            IProcessoAssinaturaDocumentoService processoAssinaturaDocumentoService,
            IGdocsWorkerExternalService gdocsWorkerExternalService,
            ILogService logService)
        {
            _solicitacaoSaidaMaterialItemRepository = solicitacaoSaidaMaterialItemRepository;
            _configuracaoRepository = configuracaoRepository;
            _processoAssinaturaDocumentoOrigemRepository = processoAssinaturaDocumentoOrigemRepository;
            _templateProcessoAssinaturaDocumentoRepository = templateProcessoAssinaturaDocumentoRepository;
            _processoAssinaturaDocumentoService = processoAssinaturaDocumentoService;
            _gdocsWorkerExternalService = gdocsWorkerExternalService;
            _logService = logService;
        }
    }
}
