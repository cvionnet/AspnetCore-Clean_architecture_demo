using Api.Middleware;
using Api.Services;
using Api.Utility;
using Application;
using Application.Contracts.Identity;
using Identity;
using Infrastructure;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using PersistenceDapper;
using Serilog;

namespace Api
{
    public class Startup
    {
        private const string CorsSettings = "Open";     //"AllowAll";

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
            // Swagger
            SwaggerConfig.Swagger_ConfigureServices(services);

            // Add services from "*.ServiceRegistration.cs" files
            services.AddApplicationServices();
            services.AddInfrastructureServices(Configuration);
            services.AddIdentityServices(Configuration);
            services.AddDapperPersistenceServices();

            // Can be used for "Domain project" AuditableEntity class (for CreatedBy or LastModifiedBy properties)
            services.AddScoped<ILoggedInUserService, LoggedInUserService>();

            services.AddControllers();

            // API Versioning
            services.AddApiVersioning(config =>
            {
                config.DefaultApiVersion = new ApiVersion(SwaggerConfig.ActiveApiVersion.major, SwaggerConfig.ActiveApiVersion.minor);    // the default API Version
                config.AssumeDefaultVersionWhenUnspecified = true;  // to use the default API version number if no version has been specified
                config.ReportApiVersions = true;
            });

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
            SwaggerConfig.Swagger_Configure(app);

            // For Error handling (call method in "/Middleware")
            app.UseCustomExceptionHandler();

            app.UseCors(CorsSettings);
            app.UseAuthorization();

            app.UseEndpoints(endpoints =>
            {
                endpoints.MapControllers();
            });
        }
    }
}
