using ICE.GDocs.Application.Core.Services;
using ICE.GDocs.Infra.CrossCutting.Models;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;
using System.Threading;
using ICE.GDocs.Domain.GDocs.Services;

namespace ICE.GDocs.Application.GDocs
{
    public interface IGravarPassoCacheAgregationAppService : IApplicationService
    {
         IGravarPassoCacheService GravarPassoCacheService { get; }
    }
}
