using HRKošarka.Domain;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace HRKošarka.Persistence.Configurations
{
    public class PlayerMatchStatsConfiguration : IEntityTypeConfiguration<PlayerMatchStats>
    {
        public void Configure(EntityTypeBuilder<PlayerMatchStats> builder)
        {
            // Configure relationships
            builder.HasOne(pms => pms.Match)
                   .WithMany(m => m.PlayerStats)
                   .HasForeignKey(pms => pms.MatchId)
                   .OnDelete(DeleteBehavior.Cascade);

            builder.HasOne(pms => pms.Player)
                   .WithMany(p => p.MatchStats)
                   .HasForeignKey(pms => pms.PlayerId)
                   .OnDelete(DeleteBehavior.Cascade);

            // Unique constraint - one stat record per player per match
            builder.HasIndex(e => new { e.MatchId, e.PlayerId })
                   .IsUnique()
                   .HasDatabaseName("IX_PlayerMatchStats_Unique");
        }
    }
}