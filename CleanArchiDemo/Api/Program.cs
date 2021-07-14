using Identity.Models;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Identity;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using Serilog;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Api
{
    public class Program
    {
        public static void Main(string[] args)
        {
            // Serilog - log errors during start-up (then, config is replaced by the one in CreateHostBuilder() )
            Log.Logger = new LoggerConfiguration()
                .WriteTo.Console()
                .CreateBootstrapLogger();
            Log.Information("Starting up!");

            try
            {
                var config = new ConfigurationBuilder()
                    .AddJsonFile("appsettings.json")
                    .Build();

                // Serilog - Read config from appsettings
                Log.Logger = new LoggerConfiguration()
                    .ReadFrom.Configuration(config)
                    .Enrich.FromLogContext()        // provides the capability to add and remove properties from the execution context
                    .WriteTo.File("Logs/log-.txt", rollingInterval: RollingInterval.Day)    // create 1 file per day
                    .CreateLogger();
                    //.CreateBootstrapLogger();

                var host = CreateHostBuilder(args).Build();

                // Identity
                using (var scope = host.Services.CreateScope())
                {
                    var services = scope.ServiceProvider;
                    var loggerFactory = services.GetRequiredService<ILoggerFactory>();

                    try
                    {
                        var userManager = services.GetRequiredService<UserManager<ApplicationUser>>();

                        //await Identity.Seed.UserCreator.SeedAsync(userManager);
                        Log.Information("Identity is ON");
                    }
                    catch (Exception ex)
                    {
                        Log.Warning(ex, "An error occured while starting the application");
                    }
                }

                host.Run();
            }
            catch (Exception ex)
            {
                Log.Fatal(ex, "An unhandled exception occured during bootstrapping");
            }
            finally
            {
                Log.CloseAndFlush();
            }
        }

        public static IHostBuilder CreateHostBuilder(string[] args) =>
            Host.CreateDefaultBuilder(args)
                .UseSerilog()       // to use Serilog by default in all logs
                .ConfigureWebHostDefaults(webBuilder =>
                {
                    webBuilder.UseStartup<Startup>();
                });
    }
}
