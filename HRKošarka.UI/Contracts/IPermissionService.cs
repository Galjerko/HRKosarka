using HRKošarka.UI.Models.UserManagement;
using System.Security.Claims;

namespace HRKošarka.UI.Contracts
{
    public interface IPermissionService
    {
        Task<UserPermissions> GetPermissionsAsync(ClaimsPrincipal user, Guid clubId);
    }
}
