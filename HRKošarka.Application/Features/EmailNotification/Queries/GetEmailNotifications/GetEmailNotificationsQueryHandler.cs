using HRKošarka.Application.Contracts.Persistence;
using HRKošarka.Application.Models.Responses;
using MediatR;

namespace HRKošarka.Application.Features.EmailNotification.Queries.GetEmailNotifications
{
    public class GetEmailNotificationsQueryHandler
        : IRequestHandler<GetEmailNotificationsQuery, PaginatedResponse<EmailNotificationDTO>>
    {
        private readonly IEmailNotificationRepository _emailNotificationRepository;

        public GetEmailNotificationsQueryHandler(IEmailNotificationRepository emailNotificationRepository)
            => _emailNotificationRepository = emailNotificationRepository;

        public async Task<PaginatedResponse<EmailNotificationDTO>> Handle(
            GetEmailNotificationsQuery request, CancellationToken cancellationToken)
            => await _emailNotificationRepository.GetPagedAsync(request, cancellationToken);
    }
}
