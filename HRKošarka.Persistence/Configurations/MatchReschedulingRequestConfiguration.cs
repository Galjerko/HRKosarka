using HRKošarka.Domain;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace HRKošarka.Persistence.Configurations
{
    public class MatchReschedulingRequestConfiguration : IEntityTypeConfiguration<MatchReschedulingRequest>
    {
        public void Configure(EntityTypeBuilder<MatchReschedulingRequest> builder)
        {
            builder.HasOne(mrr => mrr.Match)
                   .WithMany(m => m.ReschedulingRequests)
                   .HasForeignKey(mrr => mrr.MatchId)
                   .OnDelete(DeleteBehavior.Cascade);

            builder.HasIndex(e => e.MatchId)
                   .HasDatabaseName("IX_MatchReschedulingRequest_MatchId");

            builder.HasIndex(e => e.Status)
                   .HasDatabaseName("IX_MatchReschedulingRequest_Status");
        }
    }
}
