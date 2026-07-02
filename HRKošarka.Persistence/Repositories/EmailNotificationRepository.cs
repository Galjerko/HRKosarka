using HRKošarka.Application.Contracts.Persistence;
using HRKošarka.Application.Features.EmailNotification.Queries.GetEmailNotifications;
using HRKošarka.Application.Models.Responses;
using HRKošarka.Domain;
using HRKošarka.Persistence.DatabaseContext;
using Microsoft.EntityFrameworkCore;

namespace HRKošarka.Persistence.Repositories
{
    public class EmailNotificationRepository : GenericRepository<EmailNotification>, IEmailNotificationRepository
    {
        public EmailNotificationRepository(HRDatabaseContext context) : base(context) { }

        public async Task<PaginatedResponse<EmailNotificationDTO>> GetPagedAsync(
            GetEmailNotificationsQuery request, CancellationToken ct = default)
        {
            var query = _context.EmailNotifications.AsQueryable();

            if (request.NotificationType.HasValue)
                query = query.Where(e => e.NotificationType == request.NotificationType.Value);

            if (request.IsSuccessful.HasValue)
                query = query.Where(e => e.IsSuccessful == request.IsSuccessful.Value);

            var totalCount = await query.CountAsync(ct);

            var items = await query
                .OrderByDescending(e => e.SentAt)
                .Skip((request.Page - 1) * request.PageSize)
                .Take(request.PageSize)
                .Select(e => new EmailNotificationDTO
                {
                    Id = e.Id,
                    UserId = e.UserId,
                    RecipientEmail = e.RecipientEmail,
                    MatchId = e.MatchId,
                    NotificationType = e.NotificationType,
                    Subject = e.Subject,
                    Body = e.Body,
                    SentAt = e.SentAt,
                    IsSuccessful = e.IsSuccessful
                })
                .AsNoTracking()
                .ToListAsync(ct);

            return PaginatedResponse<EmailNotificationDTO>.Success(
                items,
                request.Page,
                request.PageSize,
                totalCount,
                $"Retrieved {items.Count} email notifications from page {request.Page}"
            );
        }
    }
}
