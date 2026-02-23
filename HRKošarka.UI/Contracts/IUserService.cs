using HRKošarka.UI.Services.Base;
using HRKošarka.UI.Services.Base.Common.Requests;
using HRKošarka.UI.Services.Base.Common.Responses;

namespace HRKošarka.UI.Contracts
{
    public interface IUserService
    {
        Task<PaginatedResponse<NonAdminUserDTO>> GetUsers(PaginationRequest request);

        Task<CommandResponse<bool>> RemoveClubManager(string userId);

        Task<CommandResponse<bool>> AssignClubManager(AssignClubManagerCommand command);

        Task<SimpleResponse> LockUser(string userId);
        Task<SimpleResponse> UnlockUser(string userId);

        Task<PaginatedResponse<InactiveUserDTO>> GetInactiveUsers(PaginationRequest request);

    }
}
