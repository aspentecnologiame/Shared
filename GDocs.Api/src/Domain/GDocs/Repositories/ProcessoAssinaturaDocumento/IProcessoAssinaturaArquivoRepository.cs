using ICE.GDocs.Domain.Core.Repositories;
using ICE.GDocs.Domain.Repositories;
using ICE.GDocs.Domain.Repositories.ProcessoAssinaturaDocumento;

namespace ICE.GDocs.Domain.GDocs.Repositories.ProcessoAssinaturaDocumento
{
    public interface IProcessoAssinaturaArquivoRepository : IRepository
    {
        IBinarioRepository BinarioRepository { get; }
        IArquivoRepository ArquivoRepository { get; }
        ITemplateProcessoAssinaturaDocumentoRepository TemplateProcessoAssinaturaDocumentoRepository { get; }
    }
}
