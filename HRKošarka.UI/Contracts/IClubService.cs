using HRKošarka.UI.Services.Base;
using HRKošarka.UI.Services.Base.Common.Requests;
using HRKošarka.UI.Services.Base.Common.Responses;

namespace HRKošarka.UI.Contracts
{
    public interface IClubService
    {
        Task<PaginatedResponse<ClubDTO>> GetClubs(PaginationRequest request);
        Task<QueryResponse<ClubDetailsDTO>> GetClubDetails(Guid id);
        Task<CommandResponse<Guid>> CreateClub(CreateClubCommand club);
        Task<CommandResponse<bool>> UpdateClub(Guid id, UpdateClubCommand club);
        Task<CommandResponse<bool>> DeactivateClub(Guid id);
        Task<CommandResponse<bool>> ActivateClub(Guid id);
        Task<CommandResponse<bool>> DeleteClub(Guid id);

        Task<QueryResponse<List<ClubWithoutManagerDTO>>> GetClubsWithoutManagerAsync(string? searchTerm);
    }
}
