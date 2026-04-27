using HRKošarka.Domain;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace HRKošarka.Persistence.Configurations
{
    public class PlayerTeamHistoryConfiguration : IEntityTypeConfiguration<PlayerTeamHistory>
    {
        public void Configure(EntityTypeBuilder<PlayerTeamHistory> builder)
        {
            builder.HasOne(pth => pth.Player)
                   .WithMany(p => p.TeamHistory)
                   .HasForeignKey(pth => pth.PlayerId)
                   .OnDelete(DeleteBehavior.Cascade);

            builder.HasOne(pth => pth.Team)
                   .WithMany(t => t.PlayerHistory)
                   .HasForeignKey(pth => pth.TeamId)
                   .OnDelete(DeleteBehavior.Restrict);

            builder.HasOne(pth => pth.Season)
                   .WithMany(s => s.PlayerHistory)
                   .HasForeignKey(pth => pth.SeasonId)
                   .OnDelete(DeleteBehavior.Restrict);

            builder.HasIndex(e => new { e.PlayerId, e.TeamId, e.SeasonId })
                   .IsUnique()
                   .HasFilter("[IsActive] = 1")
                   .HasDatabaseName("IX_PlayerTeamHistory_Unique");

            builder.HasIndex(e => new { e.TeamId, e.JerseyNumber })
                   .IsUnique()
                   .HasFilter("[IsActive] = 1 AND [JerseyNumber] IS NOT NULL")
                   .HasDatabaseName("IX_PlayerTeamHistory_ActiveTeamJerseyNumber");
        }
    }
}
