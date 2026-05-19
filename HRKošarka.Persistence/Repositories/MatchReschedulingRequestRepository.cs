using HRKošarka.Application.Contracts.Persistence;
using HRKošarka.Domain;
using HRKošarka.Domain.Common;
using HRKošarka.Persistence.DatabaseContext;
using Microsoft.EntityFrameworkCore;

namespace HRKošarka.Persistence.Repositories
{
    public class MatchReschedulingRequestRepository : GenericRepository<MatchReschedulingRequest>, IMatchReschedulingRequestRepository
    {
        public MatchReschedulingRequestRepository(HRDatabaseContext context) : base(context) { }

        public async Task<MatchReschedulingRequest?> GetActiveForMatchAsync(Guid matchId, CancellationToken ct = default)
        {
            return await _context.MatchReschedulingRequests
                .FirstOrDefaultAsync(r => r.MatchId == matchId
                                       && r.Status == RequestStatus.Pending
                                       && r.ExpiresAt > DateTime.UtcNow, ct);
        }

        public async Task ExpireStaleForMatchAsync(Guid matchId, CancellationToken ct = default)
        {
            var stale = await _context.MatchReschedulingRequests
                .Where(r => r.MatchId == matchId
                         && r.Status == RequestStatus.Pending
                         && r.ExpiresAt <= DateTime.UtcNow)
                .ToListAsync(ct);

            if (!stale.Any()) return;

            foreach (var req in stale)
                req.Status = RequestStatus.Expired;

            var match = await _context.Matches
                .FirstOrDefaultAsync(m => m.Id == matchId && m.SchedulingStatus == SchedulingStatus.ProposalPending, ct);
            if (match != null)
                match.SchedulingStatus = SchedulingStatus.Default;

            await _context.SaveChangesAsync(ct);
        }
    }
}
