using HRKošarka.Domain;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace HRKošarka.Persistence.Configurations
{
    public class TeamConfiguration : IEntityTypeConfiguration<Team>
    {
        public void Configure(EntityTypeBuilder<Team> builder)
        {
            builder.HasOne(t => t.Club)
                   .WithMany(c => c.Teams)
                   .HasForeignKey(t => t.ClubId)
                   .OnDelete(DeleteBehavior.Restrict);

            builder.HasOne(t => t.AgeCategory)
                   .WithMany(ac => ac.Teams)
                   .HasForeignKey(t => t.AgeCategoryId)
                   .OnDelete(DeleteBehavior.Restrict);

            builder.HasIndex(e => e.ClubId)
                   .HasDatabaseName("IX_Team_ClubId");

            builder.HasIndex(e => e.AgeCategoryId)
                   .HasDatabaseName("IX_Team_AgeCategoryId");
        }
    }
}