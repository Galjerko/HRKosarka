using HRKošarka.Domain;

namespace HRKošarka.Application.Contracts.Persistence
{
    public interface IMatchReschedulingRequestRepository : IGenericRepository<MatchReschedulingRequest>
    {
        Task<MatchReschedulingRequest?> GetActiveForMatchAsync(Guid matchId, CancellationToken ct = default);
        Task ExpireStaleForMatchAsync(Guid matchId, CancellationToken ct = default);
    }
}
