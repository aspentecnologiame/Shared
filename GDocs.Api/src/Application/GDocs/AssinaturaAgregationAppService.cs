using ICE.GDocs.Domain.Repositories.ProcessoAssinaturaDocumento;
using ICE.GDocs.Domain.Services;

namespace ICE.GDocs.Application.GDocs
{
    internal class AssinaturaAgregationAppService : IAssinaturaAgregationAppService
    {

        private readonly IArquivoRepository _arquivoRepository;
        private readonly IAssinaturaService _assinaturaService;
        private readonly IAssinaturaDocumentoRepository _assinaturaDocumentoRepository;
        private readonly ITemplateProcessoAssinaturaDocumentoRepository _templateProcessoAssinaturaDocumentoRepository;
        private readonly IDocumentoFI1548AppService _documentoFI1548AppService;

        public IArquivoRepository ArquivoRepository => _arquivoRepository;
        public IAssinaturaService AssinaturaService => _assinaturaService;
        public IAssinaturaDocumentoRepository AssinaturaDocumentoRepository => _assinaturaDocumentoRepository;
        public ITemplateProcessoAssinaturaDocumentoRepository TemplateProcessoAssinaturaDocumentoRepository => _templateProcessoAssinaturaDocumentoRepository;
        public IDocumentoFI1548AppService DocumentoFI1548AppService => _documentoFI1548AppService;

        public AssinaturaAgregationAppService
        (
            IAssinaturaService assinaturaService,
            IArquivoRepository arquivoRepository,
            IAssinaturaDocumentoRepository assinaturaDocumentoRepository,
            ITemplateProcessoAssinaturaDocumentoRepository templateProcessoAssinaturaDocumentoRepository,
            IDocumentoFI1548AppService documentoFI1548AppService)
        {
            _assinaturaService = assinaturaService;
            _arquivoRepository = arquivoRepository;
            _assinaturaDocumentoRepository = assinaturaDocumentoRepository;
            _templateProcessoAssinaturaDocumentoRepository = templateProcessoAssinaturaDocumentoRepository;
            _documentoFI1548AppService = documentoFI1548AppService;
        }
    }
}
