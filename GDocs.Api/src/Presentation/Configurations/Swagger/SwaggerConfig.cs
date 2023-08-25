using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Controllers;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Options;
using Microsoft.Extensions.PlatformAbstractions;
using Microsoft.OpenApi.Models;
using Swashbuckle.AspNetCore.Filters;
using Swashbuckle.AspNetCore.SwaggerGen;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;

namespace ICE.GDocs.Api.Configurations.Swagger
{
    internal static class SwaggerConfig
    {
        private const string BEARER = "Bearer";
        public static IServiceCollection AddSwagger(this IServiceCollection services)
        {
            _ = services
                .AddTransient<IConfigureOptions<SwaggerGenOptions>, ConfigureSwaggerOptions>()
                .AddApiVersioning(options =>
                {
                    // reporting api versions will return the headers "api-supported-versions" and "api-deprecated-versions"
                    options.ReportApiVersions = true;
                })
                .AddVersionedApiExplorer(options =>
                {
                    // add the versioned api explorer, which also adds IApiVersionDescriptionProvider service
                    // note: the specified format code will format the version as "'v'major[.minor][-status]"
                    options.GroupNameFormat = "'v'VVV";

                    // note: this option is only necessary when versioning by url segment. the SubstitutionFormat
                    // can also be used to control the format of the API version in route templates
                    options.SubstituteApiVersionInUrl = true;
                })
                .AddSwaggerGen(options =>
                {
                    options.ExampleFilters();
                    // add a custom operation filter which sets default values
                    options.OperationFilter<SwaggerDefaultValues>();

                    options.DocumentFilter<LowerCaseDocumentFilter>();

                    // integrate xml comments
                    options.IncludeXmlComments(XmlCommentsFilePath);


                    options.TagActionsBy(api =>
                    {

                        var controllerActionDescriptor = api.ActionDescriptor as ControllerActionDescriptor;
                        if (controllerActionDescriptor == null)
                            throw new InvalidOperationException("Unable to determine tag for endpoint.");

                        var apiExplorerSettingsAttribute = controllerActionDescriptor.MethodInfo.GetCustomAttributes<ApiExplorerSettingsAttribute>().FirstOrDefault();
                        if (apiExplorerSettingsAttribute != null)
                            return new[] { apiExplorerSettingsAttribute.GroupName };

                        return new[] { controllerActionDescriptor.ControllerName };
                    });

                    options.DocInclusionPredicate((_, __) =>
                    {
                        return true;

                    });


                    options.AddSecurityDefinition(BEARER, new OpenApiSecurityScheme
                    {
                        Description = @"JWT Authorization header using the Bearer scheme. 
                                        \r\n\r\n Enter 'Bearer' [space] and then your token in the text input below.
                                        \r\n\r\nExample: 'Bearer 12345abcdef'",
                        Name = "Authorization",
                        In = ParameterLocation.Header,
                        Type = SecuritySchemeType.ApiKey,
                        Scheme = BEARER
                    });

                    options.AddSecurityRequirement(new OpenApiSecurityRequirement()
                    {
                        {
                            new OpenApiSecurityScheme
                            {
                                Reference = new OpenApiReference
                                {
                                    Type = ReferenceType.SecurityScheme,
                                    Id = BEARER
                                },
                                Scheme = "oauth2",
                                Name = BEARER,
                                In = ParameterLocation.Header
                            },
                            new List<string>()
                        }
                    });

                })
                .AddSwaggerGenNewtonsoftSupport()
                .AddSwaggerExamplesFromAssemblies(typeof(SwaggerConfig).Assembly);

            return services;
        }

        private static string XmlCommentsFilePath
        {
            get
            {
                var basePath = PlatformServices.Default.Application.ApplicationBasePath;
                var fileName = typeof(Startup).GetTypeInfo().Assembly.GetName().Name + ".xml";
                return Path.Combine(basePath, fileName);
            }
        }
    }
}
