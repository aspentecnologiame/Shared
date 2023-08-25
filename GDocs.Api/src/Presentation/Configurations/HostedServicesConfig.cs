using ICE.GDocs.Api.HostedServices;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
namespace ICE.GDocs.Api.Configurations
{
    internal static class HostedServicesConfig
    {
        public static IServiceCollection AddHostedServices(this IServiceCollection services, IConfiguration configuration)
        {
            services
                .AddExpurgoUploadTempHostedService();

            return services;
        }

        private static IServiceCollection AddExpurgoUploadTempHostedService(this IServiceCollection services)
            => services.AddSingleton<Microsoft.Extensions.Hosting.IHostedService, ExpurgoUploadTempHostedService>();
    }
}
