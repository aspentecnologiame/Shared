using AutoMapper;
using Microsoft.Extensions.DependencyInjection;

namespace ICE.GDocs.Api.Configurations
{
    internal static class AutoMapperConfig
    {
        public static IServiceCollection AddAutoMapper(this IServiceCollection services)
        {
            services
                .AddSingleton<IConfigurationProvider>(provider =>
                    new MapperConfiguration(cfg =>
                    {
                        cfg.AllowNullCollections = true;
                        cfg.ConstructServicesUsing(provider.GetService);
                        cfg.AddMaps(
                            typeof(AutoMapperConfig).Assembly,
                            typeof(Application.DependencyResolver).Assembly,
                            typeof(Domain.DependencyResolver).Assembly,
                            typeof(Infra.Data.DependencyResolver).Assembly,
                            typeof(Infra.ExternalServices.DocTools.DependencyResolver).Assembly
                        );
                    })
                )
                .AddSingleton(provider =>
                    provider.GetService<IConfigurationProvider>().CreateMapper(provider.GetService)
                );

            return services;
        }
    }
}
