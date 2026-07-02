using HRKošarka.Application.Models.Responses;
using HRKošarka.Domain.Common;
using MediatR;

namespace HRKošarka.Application.Features.EmailNotification.Queries.GetEmailNotifications
{
    public class GetEmailNotificationsQuery : IRequest<PaginatedResponse<EmailNotificationDTO>>
    {
        public int Page { get; set; } = 1;
        public int PageSize { get; set; } = 20;
        public NotificationType? NotificationType { get; set; }
        public bool? IsSuccessful { get; set; }
    }
}
