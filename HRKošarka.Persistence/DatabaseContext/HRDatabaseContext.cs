using HRKošarka.Domain;
using HRKošarka.Domain.Common;
using Microsoft.AspNetCore.Http;
using Microsoft.EntityFrameworkCore;
using System.Linq.Expressions;
using System.Security.Claims;

namespace HRKošarka.Persistence.DatabaseContext
{
    public class HRDatabaseContext : DbContext
    {
        private readonly IHttpContextAccessor? _httpContextAccessor;

        public HRDatabaseContext(DbContextOptions<HRDatabaseContext> options, IHttpContextAccessor httpContextAccessor) : base(options)
        {
            _httpContextAccessor = httpContextAccessor;
        }

        public HRDatabaseContext(DbContextOptions<HRDatabaseContext> options) : base(options)
        {
            _httpContextAccessor = null;
        }

        public DbSet<Club> Clubs { get; set; }
        public DbSet<Team> Teams { get; set; }
        public DbSet<AgeCategory> AgeCategories { get; set; }
        public DbSet<Season> Seasons { get; set; }
        public DbSet<League> Leagues { get; set; }
        public DbSet<LeagueTeam> LeagueTeams { get; set; }
        public DbSet<Player> Players { get; set; }
        public DbSet<PlayerTeamHistory> PlayerTeamHistory { get; set; }
        public DbSet<TeamRepresentative> TeamRepresentatives { get; set; }
        public DbSet<Match> Matches { get; set; }
        public DbSet<MatchReschedulingRequest> MatchReschedulingRequests { get; set; }
        public DbSet<PlayerMatchStats> PlayerMatchStats { get; set; }
        public DbSet<PlayerSeasonStats> PlayerSeasonStats { get; set; }
        public DbSet<LeagueStanding> LeagueStandings { get; set; }
        public DbSet<UserFavoriteTeam> UserFavoriteTeams { get; set; }
        public DbSet<EmailNotification> EmailNotifications { get; set; }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);

            ApplySoftDeleteQueryFilter(modelBuilder);

            modelBuilder.ApplyConfigurationsFromAssembly(typeof(HRDatabaseContext).Assembly);
        }

        public override async Task<int> SaveChangesAsync(CancellationToken cancellationToken = default)
        {
            var currentUserId = _httpContextAccessor?.HttpContext?.User?.FindFirst(ClaimTypes.NameIdentifier)?.Value;
            var now = DateTime.Now;

            foreach (var entry in ChangeTracker.Entries<BaseEntity>())
            {
                switch (entry.State)
                {
                    case EntityState.Added:
                        entry.Entity.DateCreated = now;
                        entry.Entity.CreatedBy = currentUserId;
                        break;
                    case EntityState.Modified:
                        entry.Property(e => e.DateCreated).IsModified = false;
                        entry.Property(e => e.CreatedBy).IsModified = false;

                        entry.Entity.DateModified = now;
                        entry.Entity.ModifiedBy = currentUserId;
                        break;
                    case EntityState.Deleted:
                        entry.State = EntityState.Modified;
                        entry.Entity.DateDeleted = now;
                        entry.Entity.DeletedBy = currentUserId;
                        break;
                }
            }

            return await base.SaveChangesAsync(cancellationToken);
        }

        private static void ApplySoftDeleteQueryFilter(ModelBuilder modelBuilder)
        {
            foreach (var entityType in modelBuilder.Model.GetEntityTypes())
            {
                if (!typeof(BaseEntity).IsAssignableFrom(entityType.ClrType) || entityType.ClrType == typeof(BaseEntity))
                {
                    continue;
                }

                var param = Expression.Parameter(entityType.ClrType, "entity");
                var prop = Expression.PropertyOrField(param, nameof(BaseEntity.DateDeleted));
                var entityNotDeleted = Expression.Lambda(
                    Expression.Equal(prop, Expression.Constant(null, typeof(DateTime?))),
                    param
                );

                entityType.SetQueryFilter(entityNotDeleted);
            }
        }
    }
}
