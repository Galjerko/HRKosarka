using HRKošarka.Domain;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace HRKošarka.Persistence.Configurations
{
    public class EmailNotificationConfiguration : IEntityTypeConfiguration<EmailNotification>
    {
        public void Configure(EntityTypeBuilder<EmailNotification> builder)
        {
            builder.HasOne(en => en.Match)
                   .WithMany(m => m.EmailNotifications)
                   .HasForeignKey(en => en.MatchId)
                   .OnDelete(DeleteBehavior.Cascade);

            // Indexes for performance
            builder.HasIndex(e => e.UserId)
                   .HasDatabaseName("IX_EmailNotification_UserId");

            builder.HasIndex(e => e.MatchId)
                   .HasDatabaseName("IX_EmailNotification_MatchId");

            builder.HasIndex(e => e.SentAt)
                   .HasDatabaseName("IX_EmailNotification_SentAt");
        }
    }
}
