using HRKošarka.Domain;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace HRKošarka.Persistence.Configurations
{
    public class LeagueTeamConfiguration : IEntityTypeConfiguration<LeagueTeam>
    {
        public void Configure(EntityTypeBuilder<LeagueTeam> builder)
        {
            builder.HasOne(lt => lt.League)
                   .WithMany(l => l.LeagueTeams)
                   .HasForeignKey(lt => lt.LeagueId)
                   .OnDelete(DeleteBehavior.Cascade); 

            builder.HasOne(lt => lt.Team)
                   .WithMany(t => t.LeagueTeams)
                   .HasForeignKey(lt => lt.TeamId)
                   .OnDelete(DeleteBehavior.Restrict); 


            builder.HasIndex(e => new { e.LeagueId, e.TeamId })
                   .IsUnique()
                   .HasDatabaseName("IX_LeagueTeam_Unique");

            builder.HasIndex(e => e.TeamId)
                   .HasDatabaseName("IX_LeagueTeam_TeamId");
        }
    }
}
