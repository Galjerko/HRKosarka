using HRKošarka.Application.Contracts.Persistence;
using HRKošarka.Persistence.DatabaseContext;
using HRKošarka.Persistence.Repositories;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

namespace HRKošarka.Persistence
{
    public static class PersistenceServiceRegistration
    {
        public static IServiceCollection AddPersistenceServices(this IServiceCollection services, IConfiguration configuration)
        {
            services.AddDbContext<HRDatabaseContext>(options =>
            {
                options.UseSqlServer(configuration.GetConnectionString("HRDatabaseConnectionString"));
            });

            services.AddScoped(typeof(IGenericRepository<>), typeof(GenericRepository<>));

            services.AddScoped<IAgeCategoryRepository, AgeCategoryRepository>();
            services.AddScoped<IClubRepository, ClubRepository>();
            services.AddScoped<ITeamRepository, TeamRepository>();

            return services;
        }
    }
}
