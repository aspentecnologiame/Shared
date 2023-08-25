using ICE.GDocs.Domain.GDocs.Services;
using System;
using System.Collections.Generic;
using System.Text;

namespace ICE.GDocs.Application.GDocs
{
    internal class GravarPassoCacheAgregationAppService : IGravarPassoCacheAgregationAppService
    {

        private readonly IGravarPassoCacheService _gravarPassoCacheService;

        public IGravarPassoCacheService GravarPassoCacheService => _gravarPassoCacheService;

        public GravarPassoCacheAgregationAppService(IGravarPassoCacheService gravarPassoCacheService)
        {
            _gravarPassoCacheService = gravarPassoCacheService;
        }
    }
}
