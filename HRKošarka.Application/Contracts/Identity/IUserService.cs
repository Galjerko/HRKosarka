using HRKošarka.Application.Features.User.Queries.GetInactiveUsers;
using HRKošarka.Application.Features.User.Queries.GetNonAdminUsers;
using HRKošarka.Application.Models.Responses;

namespace HRKošarka.Application.Contracts.Identity
{
    public interface IUserService
    {
        Task<PaginatedResponse<NonAdminUserDTO>> GetNonAdminUsersPagedAsync(GetNonAdminUsersQuery request);

        Task<PaginatedResponse<InactiveUserDTO>> GeInactiveUsersPagedAsync(GetInactiveUsersQuery request);

        Task<SimpleResponse> LockUser(string userId);
        Task<SimpleResponse> UnlockUser(string userId);
    }
}
