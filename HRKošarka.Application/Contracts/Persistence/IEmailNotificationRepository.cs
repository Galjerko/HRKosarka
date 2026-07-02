using HRKošarka.Application.Features.EmailNotification.Queries.GetEmailNotifications;
using HRKošarka.Application.Models.Responses;
using HRKošarka.Domain;

namespace HRKošarka.Application.Contracts.Persistence
{
    public interface IEmailNotificationRepository : IGenericRepository<EmailNotification>
    {
        Task<PaginatedResponse<EmailNotificationDTO>> GetPagedAsync(
            GetEmailNotificationsQuery request, CancellationToken ct = default);
    }
}
