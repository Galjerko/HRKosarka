using HRKošarka.Application.Features.League.Queries.GetAllLeagues;
using HRKošarka.Application.Features.League.Queries.GetAvailableTeamsForLeague;
using HRKošarka.Application.Features.League.Queries.GetLeagueTeams;
using HRKošarka.Application.Features.Team.Queries.GetTeamLeagues;
using HRKošarka.Application.Models.Responses;
using HRKošarka.Domain;

namespace HRKošarka.Application.Contracts.Persistence
{
    public interface ILeagueRepository : IGenericRepository<League>
    {
        Task<League?> GetByIdWithIncludesAsync(Guid id, CancellationToken cancellationToken = default);
        Task<League?> GetLeagueWithDetailsAsync(Guid id, CancellationToken cancellationToken = default);
        Task<PaginatedResponse<League>> GetPagedWithIncludesAsync(GetLeaguesQuery request, CancellationToken cancellationToken = default);
        Task<List<LeagueTeamDTO>> GetLeagueTeamsAsync(Guid leagueId, CancellationToken cancellationToken = default);
        Task<List<AvailableTeamForLeagueDTO>> GetAvailableTeamsForLeagueAsync(Guid leagueId, string? searchTerm, CancellationToken cancellationToken = default);
        Task<LeagueTeam?> GetLeagueTeamAsync(Guid leagueId, Guid teamId, CancellationToken cancellationToken = default);
        Task<List<TeamLeagueDTO>> GetTeamLeaguesAsync(Guid teamId, CancellationToken cancellationToken = default);
        Task DeactivateAllForTeamAsync(Guid teamId, CancellationToken cancellationToken = default);
        Task DeactivateAllForLeagueAsync(Guid leagueId, CancellationToken cancellationToken = default);
    }
}
