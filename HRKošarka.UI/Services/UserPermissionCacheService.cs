using HRKošarka.UI.Contracts;
using HRKošarka.UI.Models.UserManagement;
using System.Security.Claims;

namespace HRKošarka.UI.Services
{
    public class UserPermissionCacheService : IUserPermissionCacheService, IPermissionService
    {
        private Dictionary<string, UserPermissions> _cache = new();

        public Task<UserPermissions> GetPermissionsAsync(ClaimsPrincipal user, Guid clubId)
        {
            var userId = user.FindFirst(System.Security.Claims.ClaimTypes.NameIdentifier)?.Value ?? "unknown";
            var cacheKey = $"{userId}_{clubId}";

            // Return cached permissions if they exist
            if (_cache.TryGetValue(cacheKey, out var cachedPermissions))
            {
                return Task.FromResult(cachedPermissions);
            }

            // Calculate permissions once and cache them
            var permissions = CalculatePermissions(user, clubId);
            _cache[cacheKey] = permissions;

            return Task.FromResult(permissions);
        }

        private UserPermissions CalculatePermissions(ClaimsPrincipal user, Guid clubId)
        {
            var isAdmin = user.IsInRole("Administrator");
            var isClubManager = user.IsInRole("ClubManager");

            // Get the user's managed club from JWT claim
            var userManagedClubId = GetUserManagedClubId(user);

            // Check if user can manage THIS specific club
            var canManageClub = isAdmin || (isClubManager && userManagedClubId.HasValue && userManagedClubId.Value == clubId);

            var permissions = new UserPermissions
            {
                CanCreate = canManageClub,
                CanEdit = canManageClub,
                CanDeactivate = canManageClub,
                CanDelete = isAdmin,
                ManagedClubId = userManagedClubId
            };

            return permissions;
        }

        private Guid? GetUserManagedClubId(ClaimsPrincipal user)
        {
            // Look for the ClubId claim in the JWT token
            var clubIdClaim = user.FindFirst("ClubId");

            if (clubIdClaim != null && Guid.TryParse(clubIdClaim.Value, out var clubId))
            {
                return clubId;
            }

            return null;
        }

        public void ClearCache()
        {
            _cache.Clear();
        }
    }
}
