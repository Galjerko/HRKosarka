using Blazored.LocalStorage;
using HRKošarka.UI.Contracts;
using HRKošarka.UI.Services.Base;
using HRKošarka.UI.Services.Base.Common.Responses;

namespace HRKošarka.UI.Services
{
    public class EmailNotificationService : BaseHttpService, IEmailNotificationService
    {
        public EmailNotificationService(IClient client, ILocalStorageService localStorageService)
            : base(client, localStorageService)
        {
        }

        public async Task<PaginatedResponse<EmailNotificationDTO>> GetEmailNotifications(
            int page,
            int pageSize,
            NotificationType? notificationType = null,
            bool? isSuccessful = null)
        {
            try
            {
                await AddBearerToken();
                var response = await _client.GetEmailNotificationsAsync(page, pageSize, notificationType, isSuccessful);

                return new PaginatedResponse<EmailNotificationDTO>
                {
                    Data = response.Data?.ToList() ?? new List<EmailNotificationDTO>(),
                    Pagination = response.Pagination ?? new PaginationMetadata(),
                    IsSuccess = response.IsSuccess,
                    Message = response.Message,
                    Errors = response.Errors?.ToList() ?? new List<string>()
                };
            }
            catch (ApiException ex)
            {
                return ConvertApiExceptionsToPaginated<EmailNotificationDTO>(ex);
            }
        }
    }
}
