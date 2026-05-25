using HRKošarka.Application.Features.Team.Queries.GetMyRepresentativeships;
using HRKošarka.Application.Features.Team.Queries.GetTeamRepresentatives;
using HRKošarka.Domain;

namespace HRKošarka.Application.Contracts.Persistence
{
    public interface ITeamRepresentativeRepository : IGenericRepository<TeamRepresentative>
    {
        Task<List<TeamRepresentativeDTO>> GetByTeamAsync(Guid teamId, CancellationToken ct = default);
        Task<TeamRepresentative?> GetByUserAndTeamAsync(string userId, Guid teamId, CancellationToken ct = default);
        Task<bool> IsActiveRepForTeamAsync(string userId, Guid teamId, CancellationToken ct = default);
        Task<List<TeamRepMembershipDTO>> GetActiveMembershipsByUserAsync(string userId, CancellationToken ct = default);
        Task<List<Guid>> GetActiveTeamIdsByUserAsync(string userId, CancellationToken ct = default);
        Task DeactivateAllForTeamAsync(Guid teamId, CancellationToken ct = default);
    }
}
