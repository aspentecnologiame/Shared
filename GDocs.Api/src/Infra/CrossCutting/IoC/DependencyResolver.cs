using ICE.GDocs.Application;
using ICE.GDocs.Domain;
using ICE.GDocs.Infra.Data;
using ICE.GDocs.Infra.ExternalServices.ActiveDirectory;
using ICE.GDocs.Infra.ExternalServices.ArmazenamentoTemporario;
using ICE.GDocs.Infra.ExternalServices.DocTools;
using ICE.GDocs.Infra.ExternalServices.RdlToPdfBytesConverter;
using ICE.GDocs.Infra.ExternalServices.Email;
using ICE.GDocs.Worker.Lib;
using Infra.ExternalServices.GdocsWorker;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

namespace ICE.GDocs.Infra.CrossCutting.IoC
{
    public static class DependencyResolver
    {
        public static IServiceCollection RegisterCrossCuttingDependencies(this IServiceCollection services, IConfiguration configuration)
        {
            services
                .RegisterDataDependencies(configuration)
                .RegisterDomainDependencies()
                .RegisterApplicationDependencies()
                .RegisterExternalServicesActiveDirectoryDependencies()
                .RegisterExternalServicesDocToolsDependencies(configuration)
                .RegisterExternalServicesArmazenamentoTemporarioDependencies(configuration)
                .RegisterGDocsWorkerLibDependencies(configuration, suffixName: "GdocsApi")
                .RegisterExternalServicesGdocsWorkerDependencies()
                .RegisterExternalServicesEmailDependencies(configuration)
                .RegisterExternalServicesRdlToPdfBytesConverterDependencies(configuration);

            return services;
        }
    }
}
