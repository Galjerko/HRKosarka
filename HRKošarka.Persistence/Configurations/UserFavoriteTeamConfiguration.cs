using HRKošarka.Domain;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace HRKošarka.Persistence.Configurations
{
    public class UserFavoriteTeamConfiguration : IEntityTypeConfiguration<UserFavoriteTeam>
    {
        public void Configure(EntityTypeBuilder<UserFavoriteTeam> builder)
        {
            builder.HasIndex(e => new { e.UserId, e.TeamId })
                   .IsUnique()
                   .HasDatabaseName("IX_UserFavoriteTeam_Unique");
        }
    }
}