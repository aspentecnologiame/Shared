using ICE.GDocs.Application.Core.Services;
using Microsoft.Extensions.DependencyInjection;
using System.Collections.Generic;
using System.Linq;

namespace ICE.GDocs.Application
{
    public static class DependencyResolver
    {
        public static IServiceCollection RegisterApplicationDependencies(this IServiceCollection services)
        {
            typeof(DependencyResolver).Assembly.GetTypes()
                .Where(t => t.IsClass && !t.IsAbstract && typeof(IApplicationService).IsAssignableFrom(t))
                .ForEach(t =>
                    t.GetInterfaces()
                        .Where(i => typeof(IApplicationService).IsAssignableFrom(i))
                        .ForEach(i => services.AddScoped(i, t))
                );

            return services;
        }
    }
}
