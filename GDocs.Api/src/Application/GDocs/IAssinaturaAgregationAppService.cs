using ICE.GDocs.Application.Core.Services;
using ICE.GDocs.Domain.Repositories.ProcessoAssinaturaDocumento;
using ICE.GDocs.Domain.Services;

namespace ICE.GDocs.Application.GDocs
{
    public interface IAssinaturaAgregationAppService : IApplicationService
    {
        IAssinaturaService AssinaturaService { get; }
        IArquivoRepository ArquivoRepository { get; }
        IAssinaturaDocumentoRepository AssinaturaDocumentoRepository { get; }
        ITemplateProcessoAssinaturaDocumentoRepository TemplateProcessoAssinaturaDocumentoRepository { get; }
        IDocumentoFI1548AppService DocumentoFI1548AppService { get; }
    }
}