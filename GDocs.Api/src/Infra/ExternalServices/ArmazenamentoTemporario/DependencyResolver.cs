using ICE.GDocs.Domain.ExternalServices;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

namespace ICE.GDocs.Infra.ExternalServices.ArmazenamentoTemporario
{
    public static class DependencyResolver
    {
        public static IServiceCollection RegisterExternalServicesArmazenamentoTemporarioDependencies(this IServiceCollection services, IConfiguration configuration)
             => services
                    .AddScoped<IGDocsCacheExternalService, GDocsCacheExternalService>();
    }
}
