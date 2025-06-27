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
        private readonly IHttpContextAccessor _httpContextAccessor;
        public HRDatabaseContext(DbContextOptions<HRDatabaseContext> options, IHttpContextAccessor httpContextAccessor) : base(options)
        {

            _httpContextAccessor = httpContextAccessor;

        }

        public HRDatabaseContext(DbContextOptions<HRDatabaseContext> options) : base(options)
        {
            _httpContextAccessor = null;
        }

        public DbSet<LeaveType> LeaveTypes { get; set; }
        public DbSet<LeaveAllocation> LeaveAllocations { get; set; }
        public DbSet<LeaveRequest> LeaveRequests { get; set; }
        public DbSet<Club> Clubs { get; set; }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);

            ApplySoftDeleteQueryFilter(modelBuilder);

            modelBuilder.ApplyConfigurationsFromAssembly(typeof(HRDatabaseContext).Assembly);
        }

        public override async Task<int> SaveChangesAsync(CancellationToken cancellationToken = default)
        {
            var currentUserId = _httpContextAccessor.HttpContext?.User?.FindFirst(ClaimTypes.NameIdentifier).Value;
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
                        entry.Entity.DateModified = now;
                        entry.Entity.ModifiedBy = currentUserId;
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
