using ICE.GDocs.Domain.Core.Services;
using Microsoft.Extensions.DependencyInjection;
using System.Collections.Generic;
using System.Linq;

namespace ICE.GDocs.Domain
{
    public static class DependencyResolver
    {
        public static IServiceCollection RegisterDomainDependencies(this IServiceCollection services)
            => services
                .RegisterDomainServicesDependencies();

        private static IServiceCollection RegisterDomainServicesDependencies(this IServiceCollection services)
        {
            typeof(DependencyResolver).Assembly.GetTypes()
                .Where(t => t.IsClass && !t.IsAbstract && typeof(IDomainService).IsAssignableFrom(t))
                .ForEach(t =>
                    t.GetInterfaces()
                        .Where(i => typeof(IDomainService).IsAssignableFrom(i))
                        .ForEach(i => services.AddScoped(i, t))
                );

            return services;
        }

    }
}
