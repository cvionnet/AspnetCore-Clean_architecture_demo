using Microsoft.AspNetCore.Builder;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.OpenApi.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Api.Utility
{
    public record APIVersion
    {
        public int major { get; set; }
        public int minor { get; set; }

        public APIVersion(int major, int minor)
        {
            this.major = major;
            this.minor = minor;
        }
    }

    /*********************************************************************************************************************************************/

    public static class SwaggerConfig
    {
        public static string SwaggerTitle { get; } = "CleanArchiDemo API";
        public static APIVersion ActiveApiVersion;
        private static List<APIVersion> _apiVersion = new List<APIVersion>();

        /// <summary>
        /// Initialization called in Configure() in startup
        /// </summary>
        /// <param name="services"></param>
        public static void Swagger_Configure(IApplicationBuilder app)
        {
            app.UseSwagger();
            app.UseSwaggerUI(c =>
            {
                // For API versioning
                foreach (var version in _apiVersion)
                    c.SwaggerEndpoint($"/swagger/v{version.major}.{version.minor}/swagger.json", $"{SwaggerTitle} v{version.major}.{version.minor}");
            });
        }

        /// <summary>
        /// Initialization called in ConfigureServices() in startup
        /// </summary>
        /// <param name="services"></param>
        public static void Swagger_ConfigureServices(IServiceCollection services)
        {
            // For API versioning - Add new API versions HERE
            _apiVersion.Add(new APIVersion(1, 0));
            _apiVersion.Add(new APIVersion(2, 0));
            ActiveApiVersion = _apiVersion.Last();     // set the last version as the active one

            //services.AddSwaggerDocument(configure => configure.Title = SwaggerTitle);
            services.AddSwaggerGen(c =>
            {
                c.AddSecurityDefinition("Bearer", new OpenApiSecurityScheme
                {
                    Description = @"JWT Authorization header using the Bearer scheme. 
                      Enter 'Bearer' [space] and then your token in the text input below.
                      Example: 'Bearer 12345abcdef'",
                    Name = "Authorization",
                    In = ParameterLocation.Header,
                    Type = SecuritySchemeType.ApiKey,
                    Scheme = "Bearer"
                });

                c.AddSecurityRequirement(new OpenApiSecurityRequirement()
                {{
                    new OpenApiSecurityScheme
                    {
                        Reference = new OpenApiReference
                        {
                            Type = ReferenceType.SecurityScheme,
                            Id = "Bearer"
                        },
                        Scheme = "oauth2",
                        Name = "Bearer",
                        In = ParameterLocation.Header
                    },
                    new List<string>()
                }});

                // For API versioning
                foreach (var version in _apiVersion)
                    c.SwaggerDoc($"v{version.major}.{version.minor}", new OpenApiInfo { Version = $"v{version.major}.{version.minor}", Title = SwaggerTitle });

                // Used to avoid the error "Actions require a unique method/path combination for Swagger" (when 2 methods have the same name but are in different versions)
                c.ResolveConflictingActions(apiDescriptions => apiDescriptions.First());

                c.OperationFilter<FileResultContentTypeOperationFilter>();

                // For API versioning - Remove and add the version number by the selected one
                c.OperationFilter<RemoveVersionParameterOperationFilter>();
                c.DocumentFilter<ReplaceVersionWithExactValueInPathFilter>();
            });
        }
    }
}
