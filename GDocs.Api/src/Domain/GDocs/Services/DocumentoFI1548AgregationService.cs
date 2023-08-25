using ICE.GDocs.Domain.ExternalServices;
using ICE.GDocs.Domain.GDocs.Repositories;
using ICE.GDocs.Domain.Repositories;
using ICE.GDocs.Domain.Repositories.ProcessoAssinaturaDocumento;
using System;
using System.Collections.Generic;
using System.Text;

namespace ICE.GDocs.Domain.Services
{
    public class DocumentoFI1548AgregationService : IDocumentoFI1548AgregationService
    {
        private readonly IDocumentoFI1548Repository _documentoFI1548Repository;
        private readonly IBinarioRepository _binarioRepository;
        private readonly IConfiguracaoRepository _configuracaoRepository;
        private readonly ITemplateProcessoAssinaturaDocumentoRepository _templateProcessoAssinaturaDocumentoRepository;
        private readonly ILogService _logService;
        private readonly IProcessoAssinaturaDocumentoOrigemRepository _processoAssinaturaDocumentoOrigemRepository;
        private readonly IEmailExternalService _emailExternalService;

        public IDocumentoFI1548Repository DocumentoFI1548Repository => _documentoFI1548Repository;
        public IBinarioRepository BinarioRepository => _binarioRepository;
        public IConfiguracaoRepository ConfiguracaoRepository => _configuracaoRepository;
        public ITemplateProcessoAssinaturaDocumentoRepository TemplateProcessoAssinaturaDocumentoRepository => _templateProcessoAssinaturaDocumentoRepository;
        public ILogService LogService => _logService;
        public IProcessoAssinaturaDocumentoOrigemRepository ProcessoAssinaturaDocumentoOrigemRepository => _processoAssinaturaDocumentoOrigemRepository;
        public IEmailExternalService EmailExternalService => _emailExternalService;

        public DocumentoFI1548AgregationService
        (
            IDocumentoFI1548Repository documentoFI1548Repository,
            IBinarioRepository binarioRepository,
            IConfiguracaoRepository configuracaoRepository,
            ITemplateProcessoAssinaturaDocumentoRepository templateProcessoAssinaturaDocumentoRepository,
            ILogService logService,
            IProcessoAssinaturaDocumentoOrigemRepository processoAssinaturaDocumentoOrigemRepository,
            IEmailExternalService emailExternalService
        )
        {
            _documentoFI1548Repository = documentoFI1548Repository;
            _binarioRepository = binarioRepository;
            _configuracaoRepository = configuracaoRepository;
            _templateProcessoAssinaturaDocumentoRepository = templateProcessoAssinaturaDocumentoRepository;
            _logService = logService;
            _processoAssinaturaDocumentoOrigemRepository = processoAssinaturaDocumentoOrigemRepository;
            _emailExternalService = emailExternalService;
        }
    }
}
