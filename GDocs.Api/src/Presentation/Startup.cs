using ICE.GDocs.Api.Configurations;
using ICE.GDocs.Api.Configurations.Swagger;
using ICE.GDocs.Api.Security;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.ApiExplorer;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Newtonsoft.Json.Converters;
using Newtonsoft.Json.Serialization;
using StackExchange.Redis.Extensions.Core.Configuration;

namespace ICE.GDocs.Api
{
    public class Startup
    {
        private readonly IConfiguration _configuration;
        private readonly RedisConfiguration _redisConfiguration;

        public Startup(IConfiguration configuration)
        {
            _configuration = configuration;
            _redisConfiguration = _configuration.GetSection("Redis").Get<RedisConfiguration>();
        }

        public void ConfigureServices(IServiceCollection services)
        {
            services.AddNodeServices();

            services
                .AddApplicationInsightsTelemetry()
                .AddRedisCache(_configuration)
                .AddAutoMapper()
                .AddControllers()
                .SetCompatibilityVersion(CompatibilityVersion.Version_3_0)
                .AddNewtonsoftJson(options =>
                {
                    options.SerializerSettings.ContractResolver = new CamelCasePropertyNamesContractResolver();
                    options.SerializerSettings.ReferenceLoopHandling = Newtonsoft.Json.ReferenceLoopHandling.Ignore;
                    options.SerializerSettings.NullValueHandling = Newtonsoft.Json.NullValueHandling.Ignore;
                    options.SerializerSettings.DateParseHandling = Newtonsoft.Json.DateParseHandling.None;
                    options.SerializerSettings.DateTimeZoneHandling = Newtonsoft.Json.DateTimeZoneHandling.RoundtripKind;

                    options.SerializerSettings.Converters.Add(new StringEnumConverter());
                })
                .AddXmlSerializerFormatters();

            services
                .AddCustomHealthChecks(_redisConfiguration, _configuration)
                .AddSwagger()
                .AddLogging(_configuration);

            services
                .RegisterApiDependencies(_configuration)
                .AddJwtBearerAuthentication(_redisConfiguration, _configuration);

            services
                .AddCors(options =>
                {
                    options.AddPolicy("AllowAnyOrigin",
                        builder => builder
                            .AllowAnyMethod()
                            .AllowAnyHeader()
                            .SetIsOriginAllowed(origin => true) // allow any origin
                            .AllowCredentials()
                    );
                });

            services
                .AddHttpContextAccessor();

            services.AddResponseCaching();
        }

        /// <summary>
        /// Configures the application using the provided builder, hosting environment, and API version description provider.
        /// </summary>
        /// <param name="app">The current application builder.</param>
        /// <param name="env"></param>
        /// <param name="provider">The API version descriptor provider used to enumerate defined API versions.</param>
        public void Configure(IApplicationBuilder app, IWebHostEnvironment env, IApiVersionDescriptionProvider provider)
        {
            if (env.IsDevelopment())
            {
                app.UseDeveloperExceptionPage();
            }
            else
            {
                app.UseHsts();
            }

            app.UseResponseCaching();

            app
                .UseRouting()
                .UseCors("AllowAnyOrigin")
                .UseAuthentication()
                .UseAuthorization()
                .UseMiddleware<Handlers.RequestResponseTracingHandlerMiddleware>()
                .UseHttpsRedirection()
                .UseCustomErrorHandler()
                .UseCustomHealthChecks()
                .UseEndpoints(endpoints =>
                {
                    endpoints.MapControllers();
                })
                .UseSwagger()
                .UseSwaggerUI(
                    options =>
                    {
                        foreach (var description in provider.ApiVersionDescriptions)
                        {
                            options.SwaggerEndpoint($"{_configuration.GetValue<string>("Application:VirtualDirectoryPath")}/swagger/{description.GroupName}/swagger.json", description.GroupName.ToUpperInvariant());
                        }
                    });
        }
    }
}
