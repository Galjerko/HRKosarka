using HRKošarka.Application.Contracts.Identity;
using System.Security.Claims;

namespace HRKošarka.Identity.Services
{
    public class ClubAuthorizationService : IClubAuthorizationService
    {
        public bool CanUserManageClub(ClaimsPrincipal user, Guid clubId)
        {
            // Admins can manage any club
            if (user.IsInRole("Administrator"))
            {
                return true;
            }

            // ClubManagers can only manage their assigned club
            if (user.IsInRole("ClubManager"))
            {
                var userClubId = GetUserManagedClubId(user);
                return userClubId.HasValue && userClubId.Value == clubId;
            }

            return false;
        }

        public Guid? GetUserManagedClubId(ClaimsPrincipal user)
        {
            // Look for the ClubId claim
            var clubIdClaim = user.FindFirst("ClubId");

            if (clubIdClaim != null && Guid.TryParse(clubIdClaim.Value, out var clubId))
            {
                return clubId;
            }

            return null;
        }
    }
}
