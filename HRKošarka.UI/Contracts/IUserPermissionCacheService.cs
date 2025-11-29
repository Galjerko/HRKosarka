using HRKošarka.UI.Models.UserManagement;
using System.Security.Claims;

namespace HRKošarka.UI.Contracts
{
    public interface IUserPermissionCacheService
    {
        Task<UserPermissions> GetPermissionsAsync(ClaimsPrincipal user, Guid clubId);
        void ClearCache();
    }
}
