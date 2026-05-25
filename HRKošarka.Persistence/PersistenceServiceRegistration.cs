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
            services.AddScoped<ILeagueRepository, LeagueRepository>();
            services.AddScoped<IPlayerRepository, PlayerRepository>();
            services.AddScoped<IPlayerTeamHistoryRepository, PlayerTeamHistoryRepository>();
            services.AddScoped<IMatchRepository, MatchRepository>();
            services.AddScoped<IPlayerMatchStatsRepository, PlayerMatchStatsRepository>();
            services.AddScoped<ILeagueStandingRepository, LeagueStandingRepository>();
            services.AddScoped<IPlayerSeasonStatsRepository, PlayerSeasonStatsRepository>();
            services.AddScoped<IMatchReschedulingRequestRepository, MatchReschedulingRequestRepository>();
            services.AddScoped<ITeamRepresentativeRepository, TeamRepresentativeRepository>();

            return services;
        }
    }
}
