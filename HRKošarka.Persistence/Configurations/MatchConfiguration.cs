using HRKošarka.Domain;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace HRKošarka.Persistence.Configurations
{
    public class MatchConfiguration : IEntityTypeConfiguration<Match>
    {
        public void Configure(EntityTypeBuilder<Match> builder)
        {
            builder.HasOne(m => m.HomeTeam)
                   .WithMany(t => t.HomeMatches)
                   .HasForeignKey(m => m.HomeTeamId)
                   .OnDelete(DeleteBehavior.Restrict);

            builder.HasOne(m => m.AwayTeam)
                   .WithMany(t => t.AwayMatches)
                   .HasForeignKey(m => m.AwayTeamId)
                   .OnDelete(DeleteBehavior.Restrict);

            builder.HasCheckConstraint("CK_Match_DifferentTeams", "HomeTeamId != AwayTeamId");

            builder.HasOne(m => m.League)
                   .WithMany(l => l.Matches)
                   .HasForeignKey(m => m.LeagueId)
                   .OnDelete(DeleteBehavior.Cascade);

            builder.Property(m => m.QuarterResults).HasMaxLength(500);
            builder.Property(m => m.Notes).HasMaxLength(1000);
            builder.Property(m => m.RoundName).HasMaxLength(100);
            builder.Property(m => m.VenueOverride).HasMaxLength(200);

            // Indexes for performance
            builder.HasIndex(e => e.LeagueId)
                   .HasDatabaseName("IX_Match_LeagueId");

            builder.HasIndex(e => e.HomeTeamId)
                   .HasDatabaseName("IX_Match_HomeTeamId");

            builder.HasIndex(e => e.AwayTeamId)
                   .HasDatabaseName("IX_Match_AwayTeamId");

            builder.HasIndex(e => e.ActualScheduledDate)
                   .HasDatabaseName("IX_Match_ScheduledDate");
        }
    }
}
