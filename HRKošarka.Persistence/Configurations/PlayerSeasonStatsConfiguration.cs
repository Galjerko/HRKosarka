using HRKošarka.Domain;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace HRKošarka.Persistence.Configurations
{
    public class PlayerSeasonStatsConfiguration : IEntityTypeConfiguration<PlayerSeasonStats>
    {
        public void Configure(EntityTypeBuilder<PlayerSeasonStats> builder)
        {
            builder.Property(p => p.AveragePoints)
                   .HasPrecision(5, 2);

            builder.Property(p => p.AverageThreePointers)
                   .HasPrecision(5, 2);

            builder.HasOne(pss => pss.Player)
                   .WithMany(p => p.SeasonStats)
                   .HasForeignKey(pss => pss.PlayerId)
                   .OnDelete(DeleteBehavior.Cascade);

            builder.HasOne(pss => pss.League)
                   .WithMany()
                   .HasForeignKey(pss => pss.LeagueId)
                   .OnDelete(DeleteBehavior.Cascade);

            builder.HasOne(pss => pss.Season)
                   .WithMany()
                   .HasForeignKey(pss => pss.SeasonId)
                   .OnDelete(DeleteBehavior.Restrict);

            builder.HasOne(pss => pss.Team)
                   .WithMany()
                   .HasForeignKey(pss => pss.TeamId)
                   .OnDelete(DeleteBehavior.Restrict);

            builder.HasIndex(e => new { e.PlayerId, e.LeagueId, e.SeasonId })
                   .IsUnique()
                   .HasDatabaseName("IX_PlayerSeasonStats_Unique");
        }
    }
}