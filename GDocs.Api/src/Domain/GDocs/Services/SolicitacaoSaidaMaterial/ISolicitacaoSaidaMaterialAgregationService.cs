using ICE.GDocs.Domain.Core.Services;
using ICE.GDocs.Domain.ExternalServices;
using ICE.GDocs.Domain.GDocs.Repositories.SolicitacaoSaidaMaterial;
using ICE.GDocs.Domain.Repositories;
using ICE.GDocs.Domain.Repositories.ProcessoAssinaturaDocumento;
using ICE.GDocs.Domain.Services;

namespace ICE.GDocs.Domain.GDocs.Services.SolicitacaoSaidaMaterial
{
    public interface ISolicitacaoSaidaMaterialAgregationService : IDomainService
    {
        ISolicitacaoSaidaMaterialItemRepository SolicitacaoSaidaMaterialItemRepository { get; }
        IConfiguracaoRepository ConfiguracaoRepository { get; }
        IProcessoAssinaturaDocumentoOrigemRepository ProcessoAssinaturaDocumentoOrigemRepository { get; }
        ITemplateProcessoAssinaturaDocumentoRepository TemplateProcessoAssinaturaDocumentoRepository { get; }
        IProcessoAssinaturaDocumentoService ProcessoAssinaturaDocumentoService { get; }
        IGdocsWorkerExternalService GdocsWorkerExternalService { get; }
        ILogService LogService { get; }
    }
}
