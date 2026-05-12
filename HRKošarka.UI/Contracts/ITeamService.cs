using HRKošarka.UI.Services.Base;
using HRKošarka.UI.Services.Base.Common.Requests;
using HRKošarka.UI.Services.Base.Common.Responses;

namespace HRKošarka.UI.Contracts
{
    public interface ITeamService
    {
        Task<PaginatedResponse<TeamDTO>> GetTeams(TeamPaginationRequest request);
        Task<QueryResponse<TeamDetailsDTO>> GetTeamDetails(Guid id);
        Task<QueryResponse<List<TeamRosterPlayerDTO>>> GetTeamRoster(Guid teamId);
        Task<QueryResponse<List<TeamLeagueDTO>>> GetTeamLeagues(Guid teamId);
        Task<CommandResponse<Guid>> CreateTeam(CreateTeamCommand team);
        Task<CommandResponse<bool>> UpdateTeam(Guid id, UpdateTeamCommand team);
        Task<CommandResponse<bool>> DeactivateTeam(Guid id);
        Task<CommandResponse<bool>> ActivateTeam(Guid id);
        Task<CommandResponse<bool>> DeleteTeam(Guid id);
        Task<CommandResponse<Guid>> AssignPlayerToTeam(Guid teamId, AssignPlayerToTeamCommand command);
        Task<CommandResponse<bool>> UpdatePlayerAssignmentInTeam(Guid teamId, Guid playerId, UpdatePlayerAssignmentInTeamCommand command);
        Task<CommandResponse<bool>> RemovePlayerFromTeam(Guid teamId, Guid playerId);
    }
}
