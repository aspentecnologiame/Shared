using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Logging.Configuration;
using NLog.Extensions.Logging;
using System;

namespace ICE.GDocs.Api.Configurations
{
    internal static class LoggingConfig
    {
        public static IServiceCollection AddLogging(this IServiceCollection services, IConfiguration configuration)
        {
            if (configuration == null)
                throw new ArgumentNullException(nameof(configuration));

            services
                .AddLogging(logging =>
                {
                    logging
                        .AddConfiguration(configuration.GetSection("Logging"))
                        .AddNLog(new NLogProviderOptions { CaptureMessageTemplates = true, CaptureMessageProperties = true, IncludeScopes = false });

                    if (typeof(NLog.LayoutRenderers.WindowsIdentityLayoutRenderer).IsVisible) // Workarround para carregar o WindowsIdentityLayout
                        NLog.LogManager.LoadConfiguration("Log.config");

                });

            return services;
        }
    }
}
