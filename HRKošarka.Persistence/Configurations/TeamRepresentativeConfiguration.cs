using HRKošarka.Domain;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace HRKošarka.Persistence.Configurations
{
    public class TeamRepresentativeConfiguration : IEntityTypeConfiguration<TeamRepresentative>
    {
        public void Configure(EntityTypeBuilder<TeamRepresentative> builder)
        {
            builder.HasOne(tr => tr.Team)
                   .WithMany(t => t.Representatives)
                   .HasForeignKey(tr => tr.TeamId)
                   .OnDelete(DeleteBehavior.Cascade);

            builder.HasIndex(e => new { e.UserId, e.TeamId })
                   .IsUnique()
                   .HasDatabaseName("IX_TeamRepresentative_Unique");
        }
    }
}
