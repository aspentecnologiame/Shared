using ICE.GDocs.Domain.ExternalServices;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

namespace ICE.GDocs.Infra.ExternalServices.DocTools
{
    public static class DependencyResolver
    {

        public static IServiceCollection RegisterExternalServicesDocToolsDependencies(this IServiceCollection services, IConfiguration configuration)
            => services
                 .AddScoped<IDocToolsExternalService, DocToolsExternalService>();

    }
}
