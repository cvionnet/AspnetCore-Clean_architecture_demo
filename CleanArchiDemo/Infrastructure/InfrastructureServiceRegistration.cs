using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

namespace Infrastructure
{
    public static class InfrastructureServiceRegistration
    {
        // Needed to make sure these Services have been correctly registered
        public static IServiceCollection AddInfrastructureServices(this IServiceCollection services, IConfiguration configuration)
        {
            //services.Configure<EmailSettings>(configuration.GetSection("EMailSettings"));

            //services.AddTransient<IEmailService, EmailService>();
            //services.AddTransient<ICsvExporter, CsvExporter>();

            return services;
        }

    }
}
