using HealthChecks.UI.Client;
using ICE.GDocs.Common.MessageBus;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Diagnostics.HealthChecks;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using StackExchange.Redis.Extensions.Core.Configuration;
using System;

namespace ICE.GDocs.Api.Configurations
{
    internal static class HealthChecksConfig
    {
        public static IServiceCollection AddCustomHealthChecks(this IServiceCollection services, RedisConfiguration redisConfiguration, IConfiguration configuration)
        {
            var databaseTags = new[] { "db", "sqlserver" };

            var healthChecksBuilder = services
                .AddHealthChecks();

            var connectionString = configuration.GetConnectionString("GDocs");

            var gDocsSqlConnectionStringBuilder = new System.Data.SqlClient.SqlConnectionStringBuilder(connectionString);

            healthChecksBuilder
                .AddSqlServer(
                    connectionString: connectionString,
                    name: $"{gDocsSqlConnectionStringBuilder.DataSource}:{gDocsSqlConnectionStringBuilder.InitialCatalog}",
                    tags: databaseTags
                )
                .AddUrlGroup(configuration.GetValue<Uri>("Infra:ExternalServices:DocToolsApi:HealthCheckUrl"), "DocToolsApi")
                .AddRabbitMQ(
                    rabbitConnectionString: configuration.GetMessageBusConnectionString(),
                    sslOption: new RabbitMQ.Client.SslOption { Enabled = false },
                    name: $"{configuration.GetValue("MessageBus:HostAddress", "RabbitMQ")} - MessageBus",
                    tags: new[] { "MessageBus", "RabbitMQ" }
                );

            foreach (var host in redisConfiguration.Hosts)
            {
                healthChecksBuilder.AddRedis(
                    redisConnectionString: $"{host.Host}:{host.Port},password={redisConfiguration.Password}",
                    name: $"{host.Host}:{host.Port} - Redis",
                    tags: new[] { "cache", "redis" }
                );
            }

            return services;
        }

        public static IApplicationBuilder UseCustomHealthChecks(this IApplicationBuilder app)
        {
            app
                .UseHealthChecks("/health", new HealthCheckOptions
                {
                    AllowCachingResponses = true,
                    ResponseWriter = UIResponseWriter.WriteHealthCheckUIResponse
                });

            return app;
        }
    }
}
