using ICE.GDocs.Application.Core.Services;
using ICE.GDocs.Domain.Services;
using Microsoft.Extensions.Configuration;
using System;
using System.Collections.Generic;
using System.Text;

namespace ICE.GDocs.Application.GDocs.SolicitacaoSaidaMaterial
{
    public interface ISolicitacaoSaidaMaterialAgregationAppService : IApplicationService
    {
        ILogService LogService { get; }
        IConfiguration Configuration { get; }
    }
}
