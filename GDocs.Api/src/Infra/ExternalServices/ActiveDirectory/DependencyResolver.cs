using ICE.GDocs.Domain.ExternalServices;
using Microsoft.Extensions.DependencyInjection;

namespace ICE.GDocs.Infra.ExternalServices.ActiveDirectory
{
    public static class DependencyResolver
    {
        public static IServiceCollection RegisterExternalServicesActiveDirectoryDependencies(this IServiceCollection services)
        {
            services
                .AddScoped<IActiveDirectoryExternalService, ActiveDirectoryExternalService>();

            return services;
        }
    }
}
