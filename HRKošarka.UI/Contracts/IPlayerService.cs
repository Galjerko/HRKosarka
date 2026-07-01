using HRKošarka.UI.Services.Base;
using HRKošarka.UI.Services.Base.Common;
using HRKošarka.UI.Services.Base.Common.Requests;
using HRKošarka.UI.Services.Base.Common.Responses;

namespace HRKošarka.UI.Contracts
{
    public interface IPlayerService
    {
        Task<PaginatedResponse<PlayerDTO>> GetPlayers(PaginationRequest request);
        Task<QueryResponse<PlayerDetailsDTO>> GetPlayerDetails(Guid id);
        Task<QueryResponse<List<AvailablePlayerDTO>>> GetAvailablePlayers(Guid teamId, string? searchTerm);
        Task<QueryResponse<List<PlayerAssignmentDTO>>> GetPlayerAssignments(Guid playerId);
        Task<QueryResponse<List<AvailableTeamDTO>>> GetAvailableTeamsForPlayer(Guid playerId, string? searchTerm);
        Task<CommandResponse<Guid>> CreatePlayer(CreatePlayerCommand player);
        Task<CommandResponse<bool>> UpdatePlayer(Guid id, UpdatePlayerCommand player);
        Task<CommandResponse<bool>> DeactivatePlayer(Guid id);
        Task<CommandResponse<bool>> ActivatePlayer(Guid id);
        Task<CommandResponse<bool>> DeletePlayer(Guid id);
        Task<QueryResponse<List<PlayerSeasonGroupDTO>>> GetPlayerSeasonStats(Guid playerId);
        Task<QueryResponse<List<PlayerSeasonGroupDTO>>> GetPlayerPlayoffStats(Guid playerId);
        Task<QueryResponse<List<PlayerCareerItemDTO>>> GetPlayerCareer(Guid playerId);
    }
}
