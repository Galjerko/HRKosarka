using HRKošarka.UI.Services.Base;
using HRKošarka.UI.Services.Base.Common.Requests;
using HRKošarka.UI.Services.Base.Common.Responses;

namespace HRKošarka.UI.Contracts
{
    public interface ITeamService
    {
        Task<PaginatedResponse<TeamDTO>> GetTeams(TeamPaginationRequest request);
        Task<QueryResponse<TeamDetailsDTO>> GetTeamDetails(Guid id);
        Task<CommandResponse<Guid>> CreateTeam(CreateTeamCommand team);
        Task<CommandResponse<bool>> UpdateTeam(Guid id, UpdateTeamCommand team);
        Task<CommandResponse<bool>> DeactivateTeam(Guid id);
        Task<CommandResponse<bool>> DeleteTeam(Guid id);
    }
}
