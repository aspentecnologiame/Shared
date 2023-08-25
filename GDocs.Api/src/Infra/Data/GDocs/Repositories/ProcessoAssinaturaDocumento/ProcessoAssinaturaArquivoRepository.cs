using ICE.GDocs.Domain.Core.Uow;
using ICE.GDocs.Domain.Database;
using ICE.GDocs.Domain.GDocs.Repositories.ProcessoAssinaturaDocumento;
using ICE.GDocs.Domain.Repositories;
using ICE.GDocs.Domain.Repositories.ProcessoAssinaturaDocumento;
using ICE.GDocs.Infra.Data.Core.Repositories;

namespace ICE.GDocs.Infra.Data.GDocs.Repositories.ProcessoAssinaturaDocumento
{
    internal class ProcessoAssinaturaArquivoRepository : Repository, IProcessoAssinaturaArquivoRepository
    {
        public IBinarioRepository BinarioRepository { get; }
        public IArquivoRepository ArquivoRepository { get; }
        public ITemplateProcessoAssinaturaDocumentoRepository TemplateProcessoAssinaturaDocumentoRepository { get; }

        public ProcessoAssinaturaArquivoRepository(
            IBinarioRepository binarioRepository,
            IArquivoRepository arquivoRepository,
            ITemplateProcessoAssinaturaDocumentoRepository templateProcessoAssinaturaDocumentoRepository,
            IGDocsDatabase db,
            IUnitOfWork unitOfWork) : base(db, unitOfWork)
        {
            BinarioRepository = binarioRepository;
            ArquivoRepository = arquivoRepository;
            TemplateProcessoAssinaturaDocumentoRepository = templateProcessoAssinaturaDocumentoRepository;
        }        
    }
}
