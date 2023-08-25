using ICE.GDocs.Infra.CrossCutting.IoC;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

namespace ICE.GDocs.Api
{
    public static class DependencyResolver
    {
        /// <summary>
        /// 
        /// </summary>
        /// <param name="services"></param>
        /// <param name="configuration"></param>
        /// <returns></returns>
        public static IServiceCollection RegisterApiDependencies(this IServiceCollection services, IConfiguration configuration)
            => services
                .RegisterCrossCuttingDependencies(configuration);
    }
}
