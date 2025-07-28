using HRKošarka.Domain;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace HRKošarka.Persistence.Configurations
{
    public class LeagueStandingConfiguration : IEntityTypeConfiguration<LeagueStanding>
    {
        public void Configure(EntityTypeBuilder<LeagueStanding> builder)
        {
            builder.HasOne(ls => ls.League)
                   .WithMany(l => l.Standings)
                   .HasForeignKey(ls => ls.LeagueId)
                   .OnDelete(DeleteBehavior.Cascade); 

            builder.HasOne(ls => ls.Team)
                   .WithMany() 
                   .HasForeignKey(ls => ls.TeamId)
                   .OnDelete(DeleteBehavior.Restrict);

            builder.HasOne(ls => ls.Season)
                   .WithMany() 
                   .HasForeignKey(ls => ls.SeasonId)
                   .OnDelete(DeleteBehavior.Restrict); 

            builder.HasIndex(e => new { e.LeagueId, e.TeamId, e.SeasonId })
                   .IsUnique()
                   .HasDatabaseName("IX_LeagueStanding_Unique");

            builder.HasIndex(e => new { e.LeagueId, e.Position })
                   .HasDatabaseName("IX_LeagueStanding_Position");
        }
    }
}
