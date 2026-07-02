using HRKošarka.UI.Services.Base;
using HRKošarka.UI.Services.Base.Common.Responses;

namespace HRKošarka.UI.Contracts
{
    public interface IEmailNotificationService
    {
        Task<PaginatedResponse<EmailNotificationDTO>> GetEmailNotifications(
            int page,
            int pageSize,
            NotificationType? notificationType = null,
            bool? isSuccessful = null);
    }
}
