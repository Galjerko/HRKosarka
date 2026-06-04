using HRKošarka.UI.Services.Base;
using HRKošarka.UI.Services.Base.Common.Requests;
using HRKošarka.UI.Services.Base.Common.Responses;

namespace HRKošarka.UI.Contracts
{
    public interface ILeagueService
    {
        Task<PaginatedResponse<LeagueDTO>> GetLeagues(
            PaginationRequest request,
            Guid? seasonId = null,
            Guid? ageCategoryId = null,
            Gender? gender = null,
            CompetitionType? competitionType = null,
            bool? isActive = null);

        Task<QueryResponse<LeagueDetailsDTO>> GetLeagueById(Guid id);
        Task<CommandResponse<Guid>> CreateLeague(CreateLeagueCommand command);
        Task<CommandResponse<bool>> UpdateLeague(Guid id, UpdateLeagueCommand command);
        Task<CommandResponse<bool>> DeactivateLeague(Guid id);
        Task<CommandResponse<bool>> ActivateLeague(Guid id);
        Task<CommandResponse<bool>> DeleteLeague(Guid id);
        Task<QueryResponse<List<LeagueTeamDTO>>> GetLeagueTeams(Guid leagueId);
        Task<QueryResponse<List<AvailableTeamForLeagueDTO>>> GetAvailableTeamsForLeague(Guid leagueId, string? searchTerm = null);
        Task<CommandResponse<Guid>> RegisterTeamInLeague(Guid leagueId, RegisterTeamInLeagueCommand command);
        Task<CommandResponse<bool>> RemoveTeamFromLeague(Guid leagueId, Guid teamId);
        Task<QueryResponse<List<LeagueBreakDTO>>> GetLeagueBreaks(Guid leagueId);
        Task<CommandResponse<Guid>> AddLeagueBreak(Guid leagueId, AddLeagueBreakCommand command);
        Task<CommandResponse<bool>> RemoveLeagueBreak(Guid leagueId, Guid breakId);
        Task<CommandResponse<int>> GenerateLeagueSchedule(Guid leagueId);
        Task<QueryResponse<List<LeagueRoundDTO>>> GetLeagueSchedule(Guid leagueId);
        Task<QueryResponse<List<FeaturedLeagueRoundDTO>>> GetFeaturedLeagueMatches();
        Task<QueryResponse<LeagueStandingsDTO>> GetLeagueStandings(Guid leagueId);
        Task<QueryResponse<List<LeaguePlayerStatDTO>>> GetLeagueLeaderboard(Guid leagueId, string? sortBy = null, string? sortDirection = null);
    }
}
