using ICE.GDocs.Domain.Core.Repositories;
using ICE.GDocs.Domain.Core.Uow;
using ICE.GDocs.Domain.Database;
using ICE.GDocs.Infra.Data.Core.UoW;
using ICE.GDocs.Infra.Data.Database;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using System.Collections.Generic;
using System.Linq;

namespace ICE.GDocs.Infra.Data
{
    public static class DependencyResolver
    {
        public static IServiceCollection RegisterDataDependencies(this IServiceCollection services, IConfiguration configuration)
        {
            services
                .AddScoped<IUnitOfWork, UnitOfWork>()
                .AddScoped<IGDocsDatabase>(p => new GDocsDatabase(configuration.GetConnectionString("GDocs")))
                .RegisterRepositoryDependencies();

            return services;
        }

        public static IServiceCollection RegisterRepositoryDependencies(this IServiceCollection services)
        {
            typeof(DependencyResolver).Assembly.GetTypes()
               .Where(t => t.IsClass && !t.IsAbstract && typeof(IRepository).IsAssignableFrom(t))
               .ForEach(t =>
                   t.GetInterfaces()
                       .Where(i => typeof(IRepository).IsAssignableFrom(i))
                       .ForEach(i => services.AddScoped(i, t))
               );

            return services;
        }
    }
}