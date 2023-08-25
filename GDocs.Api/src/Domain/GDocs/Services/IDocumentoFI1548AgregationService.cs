using ICE.GDocs.Domain.Core.Services;
using ICE.GDocs.Domain.ExternalServices;
using ICE.GDocs.Domain.GDocs.Repositories;
using ICE.GDocs.Domain.Repositories;
using ICE.GDocs.Domain.Repositories.ProcessoAssinaturaDocumento;
using System;
using System.Collections.Generic;
using System.Text;

namespace ICE.GDocs.Domain.Services
{
    public interface IDocumentoFI1548AgregationService : IDomainService
    {
        IDocumentoFI1548Repository DocumentoFI1548Repository { get; }
        IBinarioRepository BinarioRepository { get; }
        IConfiguracaoRepository ConfiguracaoRepository { get; }
        ITemplateProcessoAssinaturaDocumentoRepository TemplateProcessoAssinaturaDocumentoRepository { get; }
        ILogService LogService { get; }
        IProcessoAssinaturaDocumentoOrigemRepository ProcessoAssinaturaDocumentoOrigemRepository { get; }
        IEmailExternalService EmailExternalService { get; }
    }
}
