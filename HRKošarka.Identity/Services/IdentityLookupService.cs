using HRKošarka.Application.Contracts.Identity;
using HRKošarka.Identity.Models;
using Microsoft.AspNetCore.Identity;

namespace HRKošarka.Identity.Services
{
    public class IdentityLookupService : IIdentityLookupService
    {
        private readonly UserManager<ApplicationUser> _userManager;

        public IdentityLookupService(UserManager<ApplicationUser> userManager)
        {
            _userManager = userManager;
        }

        public async Task<string?> GetEmailByUserIdAsync(string userId, CancellationToken cancellationToken = default)
        {
            var user = await _userManager.FindByIdAsync(userId);
            return user?.Email;
        }

        public async Task<List<string>> GetUserIdsInRoleAsync(string role, CancellationToken cancellationToken = default)
        {
            var users = await _userManager.GetUsersInRoleAsync(role);
            return users.Select(u => u.Id).ToList();
        }
    }
}
