using ICE.GDocs.Domain.ExternalServices;
using Microsoft.Extensions.DependencyInjection;

namespace Infra.ExternalServices.GdocsWorker
{
    public static class DependencyResolver
    {
        public static IServiceCollection RegisterExternalServicesGdocsWorkerDependencies(this IServiceCollection services)
            => services
                 .AddScoped<IGdocsWorkerExternalService, GdocsWorkerExternalService>();
    }
}
