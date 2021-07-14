using Api.Middleware;
using Api.Services;
using Api.Utility;
using Application;
using Application.Contracts.Identity;
using Identity;
using Infrastructure;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.OpenApi.Models;
using Serilog;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Api
{
    public class Startup
    {
        private const string CorsSettings = "Open";     //"AllowAll";
        private const string SwaggerTitle = "CleanArchiDemo API";
        private const string SwaggerVersion = "v1";

        // To read configuration file "appsettings.json"
        public Startup(IConfiguration configuration)
        {
            Configuration = configuration;
        }
        public IConfiguration Configuration { get; }

        // This method gets called by the runtime. Use this method to add services to the container.
        // For more information on how to configure your application, visit https://go.microsoft.com/fwlink/?LinkID=398940
        public void ConfigureServices(IServiceCollection services)
        {
            AddSwagger(services);

            // Add services from "*.ServiceRegistration.cs" files
            services.AddApplicationServices();
            services.AddInfrastructureServices(Configuration);
            services.AddIdentityServices(Configuration);
            //services.AddDapperPersistenceServices();

            // Can be used for "Domain project" AuditableEntity class (for CreatedBy or LastModifiedBy properties)
            services.AddScoped<ILoggedInUserService, LoggedInUserService>();

            services.AddControllers();

            services.AddCors(options =>
            {
                options.AddPolicy(CorsSettings, builder => builder.AllowAnyOrigin().AllowAnyHeader().AllowAnyMethod());
            });
        }

        // This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
        public void Configure(IApplicationBuilder app, IWebHostEnvironment env)
        {
            if (env.IsDevelopment())
            {
                app.UseDeveloperExceptionPage();
            }

            app.UseHttpsRedirection();
            app.UseRouting();
            app.UseAuthentication();

            // To write request completion events (https://github.com/serilog/serilog-aspnetcore#request-logging)
            app.UseSerilogRequestLogging();

            // Swagger
            app.UseSwagger();
            app.UseSwaggerUI(c =>
            {
                c.SwaggerEndpoint($"/swagger/{SwaggerVersion}/swagger.json", SwaggerTitle);
            });

            // For Error handling (call method in "/Middleware")
            app.UseCustomExceptionHandler();

            app.UseCors(CorsSettings);
            app.UseAuthorization();

            app.UseEndpoints(endpoints =>
            {
                endpoints.MapControllers();
            });
        }

        private void AddSwagger(IServiceCollection services)
        {
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

                c.SwaggerDoc(SwaggerVersion, new OpenApiInfo
                {
                    Version = SwaggerVersion,
                    Title = SwaggerTitle,
                });

                c.OperationFilter<FileResultContentTypeOperationFilter>();
            });
        }
    }
}
