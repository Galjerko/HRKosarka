using System.Security.Claims;

namespace HRKošarka.Application.Contracts.Identity
{
    public interface IClubAuthorizationService
    {
        bool CanUserManageClub(ClaimsPrincipal user, Guid clubId);
        Guid? GetUserManagedClubId(ClaimsPrincipal user);
    }
}
