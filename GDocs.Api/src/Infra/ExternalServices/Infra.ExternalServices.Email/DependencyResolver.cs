using ICE.GDocs.Domain.ExternalServices;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

namespace ICE.GDocs.Infra.ExternalServices.Email
{
    public static class DependencyResolver
    {
        public static IServiceCollection RegisterExternalServicesEmailDependencies(this IServiceCollection services, IConfiguration configuration)
            => services
                 .AddScoped<IEmailExternalService, EmailService>();
    }
}
