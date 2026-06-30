using HRKošarka.Domain;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace HRKošarka.Persistence.Configurations
{
    public class PlayoffSeriesConfiguration : IEntityTypeConfiguration<PlayoffSeries>
    {
        public void Configure(EntityTypeBuilder<PlayoffSeries> builder)
        {
            builder.HasOne(s => s.League)
                   .WithMany(l => l.PlayoffSeries)
                   .HasForeignKey(s => s.LeagueId)
                   .OnDelete(DeleteBehavior.Cascade);

            builder.HasOne(s => s.HomeTeam)
                   .WithMany()
                   .HasForeignKey(s => s.HomeTeamId)
                   .OnDelete(DeleteBehavior.Restrict)
                   .IsRequired(false);

            builder.HasOne(s => s.AwayTeam)
                   .WithMany()
                   .HasForeignKey(s => s.AwayTeamId)
                   .OnDelete(DeleteBehavior.Restrict)
                   .IsRequired(false);

            builder.HasOne(s => s.HomeFeederSeries)
                   .WithMany()
                   .HasForeignKey(s => s.HomeFeederSeriesId)
                   .OnDelete(DeleteBehavior.Restrict)
                   .IsRequired(false);

            builder.HasOne(s => s.AwayFeederSeries)
                   .WithMany()
                   .HasForeignKey(s => s.AwayFeederSeriesId)
                   .OnDelete(DeleteBehavior.Restrict)
                   .IsRequired(false);

            builder.HasCheckConstraint("CK_PlayoffSeries_WinsNeeded", "WinsNeeded BETWEEN 2 AND 4");

            // Unique: only one series per position per round per league
            builder.HasIndex(s => new { s.LeagueId, s.RoundNumber, s.SeriesNumber })
                   .IsUnique()
                   .HasDatabaseName("IX_PlayoffSeries_LeagueId_RoundNumber_SeriesNumber");

            builder.HasIndex(s => s.LeagueId)
                   .HasDatabaseName("IX_PlayoffSeries_LeagueId");

            builder.HasIndex(s => new { s.LeagueId, s.RoundNumber })
                   .HasDatabaseName("IX_PlayoffSeries_LeagueId_RoundNumber");

            // Ignore computed properties — not stored in DB
            builder.Ignore(s => s.HomeWins);
            builder.Ignore(s => s.AwayWins);
        }
    }
}
