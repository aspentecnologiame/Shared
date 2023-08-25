using ICE.GDocs.Domain.Services;
using Microsoft.Extensions.Configuration;
using System;
using System.Collections.Generic;
using System.Text;

namespace ICE.GDocs.Application.GDocs.SolicitacaoSaidaMaterial
{
    public class SolicitacaoSaidaMaterialAgregationAppService : ISolicitacaoSaidaMaterialAgregationAppService
    {
        private readonly IConfiguration _configuration;
        private readonly ILogService _logService;

        public IConfiguration Configuration => _configuration;
        public ILogService LogService => _logService;

        public SolicitacaoSaidaMaterialAgregationAppService(IConfiguration configuration, ILogService logService)
        {
            _configuration = configuration;
            _logService = logService;
        }
    }
}
