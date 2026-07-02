using HRKošarka.Domain.Common;

namespace HRKošarka.Application.Features.EmailNotification.Queries.GetEmailNotifications
{
    public class EmailNotificationDTO
    {
        public Guid Id { get; set; }
        public string UserId { get; set; } = string.Empty;
        public string? RecipientEmail { get; set; }
        public Guid? MatchId { get; set; }
        public NotificationType NotificationType { get; set; }
        public string Subject { get; set; } = string.Empty;
        public string Body { get; set; } = string.Empty;
        public DateTime SentAt { get; set; }
        public bool IsSuccessful { get; set; }
    }
}
