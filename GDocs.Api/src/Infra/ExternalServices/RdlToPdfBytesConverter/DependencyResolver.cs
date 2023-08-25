using ICE.GDocs.Domain.ExternalServices;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

namespace ICE.GDocs.Infra.ExternalServices.RdlToPdfBytesConverter
{
    public static class DependencyResolver
    {
        public static IServiceCollection RegisterExternalServicesRdlToPdfBytesConverterDependencies(this IServiceCollection services, IConfiguration configuration)
            => services
                 .AddScoped<IRdlToPdfBytesConverterExternalService, RdlToPdfBytesConverterExternalService>();
    }
}
