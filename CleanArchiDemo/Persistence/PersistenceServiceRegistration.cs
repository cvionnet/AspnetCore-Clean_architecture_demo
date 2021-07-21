using Application.Contracts.Persistence;
using Microsoft.Extensions.DependencyInjection;
using PersistenceDapper.Repositories;

namespace PersistenceDapper
{
    public static class PersistenceServiceRegistration
    {
        public static IServiceCollection AddDapperPersistenceServices(this IServiceCollection services)
        {
//            services.AddScoped(typeof(IAsyncRepository<>), typeof(BaseRepository<>));
            
            services.AddScoped<ICompanyRepository, CompanyRepository>();
            services.AddScoped<IUnitOfWork, UnitOfWork>();

            return services;
        }
    }
}
