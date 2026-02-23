using HRKošarka.Application.Contracts.Identity;
using HRKošarka.Identity.Models;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;

namespace HRKošarka.Identity.Services
{
    public class UserReadService : IUserReadService
    {
        private readonly UserManager<ApplicationUser> _userManager;

        public UserReadService(UserManager<ApplicationUser> userManager)
        {
            _userManager = userManager;
        }

        public async Task<List<Guid>> GetManagedClubIdsAsync(CancellationToken cancellationToken = default)
        {
            return await _userManager.Users
                .Where(u => u.ManagedClubId != null)
                .Select(u => u.ManagedClubId!.Value)
                .Distinct()
                .ToListAsync(cancellationToken);
        }
    }
}
