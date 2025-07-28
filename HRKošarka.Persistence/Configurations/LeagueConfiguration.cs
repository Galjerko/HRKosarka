using HRKošarka.Domain;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace HRKošarka.Persistence.Configurations
{
    public class LeagueConfiguration : IEntityTypeConfiguration<League>
    {
        public void Configure(EntityTypeBuilder<League> builder)
        {
            builder.HasOne(l => l.Season)
                   .WithMany(s => s.Leagues)
                   .HasForeignKey(l => l.SeasonId)
                   .OnDelete(DeleteBehavior.Restrict);

            builder.HasOne(l => l.AgeCategory)
                   .WithMany(ac => ac.Leagues)
                   .HasForeignKey(l => l.AgeCategoryId)
                   .OnDelete(DeleteBehavior.Restrict); 

            builder.HasIndex(e => e.SeasonId)
                   .HasDatabaseName("IX_League_SeasonId");

            builder.HasIndex(e => e.AgeCategoryId)
                   .HasDatabaseName("IX_League_AgeCategoryId");
        }
    }
}