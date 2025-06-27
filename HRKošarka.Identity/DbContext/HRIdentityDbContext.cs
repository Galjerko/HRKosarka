using HRKošarka.Identity.Models;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;
using System.Security.Claims;

namespace HRKošarka.Identity.DbContext
{
    public class HRIdentityDbContext : IdentityDbContext<ApplicationUser>
    {
        private readonly IHttpContextAccessor _httpContextAccessor;
        public HRIdentityDbContext(
            DbContextOptions<HRIdentityDbContext> options,
            IHttpContextAccessor httpContextAccessor) : base(options)
        {
            _httpContextAccessor = httpContextAccessor;
        }

        public HRIdentityDbContext(DbContextOptions<HRIdentityDbContext> options) : base(options)
        {
            _httpContextAccessor = null; 
        }

        public override async Task<int> SaveChangesAsync(CancellationToken cancellationToken = default)
        {
            var currentUserId = _httpContextAccessor.HttpContext?.User?.FindFirstValue(ClaimTypes.NameIdentifier);
            var now = DateTime.Now;

            foreach (var entry in ChangeTracker.Entries<ApplicationUser>())
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
        protected override void OnModelCreating(ModelBuilder builder)
        {
            base.OnModelCreating(builder);
            builder.ApplyConfigurationsFromAssembly(typeof(HRIdentityDbContext).Assembly);
        }
    }
}
