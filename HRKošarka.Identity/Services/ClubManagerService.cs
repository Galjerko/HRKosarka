using HRKošarka.Application.Contracts.Identity;
using HRKošarka.Application.Models.Identity;
using HRKošarka.Identity.Models;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using System.Security.Claims;

namespace HRKošarka.Identity.Services
{
    public class ClubManagerService : IClubManagerService
    {
        private readonly UserManager<ApplicationUser> _userManager;

        public ClubManagerService(UserManager<ApplicationUser> userManager)
        {
            _userManager = userManager;
        }

        public async Task<ClubManagerResult> AssignClubManager(string userId, Guid clubId)
        {
            var user = await _userManager.FindByIdAsync(userId);
            if (user == null)
            {
                return ClubManagerResult.Failure("User not found");
            }

            // Check if club already has a manager (excluding current user)
            var existingClubManagers = await _userManager.Users
                .Where(u => u.ManagedClubId == clubId)
                .ToListAsync();

            var currentClubManager = existingClubManagers.FirstOrDefault(u => u.Id != userId);
            if (currentClubManager != null)
            {
                return ClubManagerResult.Failure($"Club already has a ClubManager: {currentClubManager.UserName} ({currentClubManager.Email})");
            }

            // Check if user is already managing another club
            if (user.ManagedClubId.HasValue && user.ManagedClubId.Value != clubId)
            {
                return ClubManagerResult.Failure($"User is already assigned to another club (ClubId: {user.ManagedClubId})");
            }

            // Check if this user already has ClubManager role. If not, add it.
            if (!await _userManager.IsInRoleAsync(user, "ClubManager"))
            {
                var roleResult = await _userManager.AddToRoleAsync(user, "ClubManager");
                if (!roleResult.Succeeded)
                {
                    return ClubManagerResult.Failure("Failed to assign ClubManager role");
                }
            }

            // Get all claims for this user and find old ClubId claim (if exists)
            var existingClaims = await _userManager.GetClaimsAsync(user);
            var existingClubClaim = existingClaims.FirstOrDefault(c => c.Type == CustomClaimTypes.ClubId);
            if (existingClubClaim != null)
            {
                await _userManager.RemoveClaimAsync(user, existingClubClaim);
            }

            // Add new ClubId claim for this user
            var claimResult = await _userManager.AddClaimAsync(
                user,
                new Claim(CustomClaimTypes.ClubId, clubId.ToString()));

            if (!claimResult.Succeeded)
            {
                return ClubManagerResult.Failure("Failed to assign club to manager");
            }

            // Update user database record (for admin queries)
            user.ManagedClubId = clubId;
            await _userManager.UpdateAsync(user);

            return ClubManagerResult.Success();
        }




        public async Task<ClubManagerResult> RemoveClubManager(string userId)
        {
            var user = await _userManager.FindByIdAsync(userId);
            if (user == null)
            {
                return ClubManagerResult.Failure("User not found");
            }

            // Remove ClubId claim
            var claims = await _userManager.GetClaimsAsync(user);
            var clubClaim = claims.FirstOrDefault(c => c.Type == CustomClaimTypes.ClubId);
            if (clubClaim != null)
            {
                await _userManager.RemoveClaimAsync(user, clubClaim);
            }

            // Remove role
            await _userManager.RemoveFromRoleAsync(user, "ClubManager");

            // Clear from entity
            user.ManagedClubId = null;
            await _userManager.UpdateAsync(user);

            // Lock account
            user.LockoutEnabled = true;
            user.LockoutEnd = DateTimeOffset.UtcNow.AddYears(100);

            var updateResult = await _userManager.UpdateAsync(user);
            if (!updateResult.Succeeded)
            {
                return ClubManagerResult.Failure("Failed to update user while removing club manager role");
            }

            return ClubManagerResult.Success();
        }


        public async Task<Guid?> GetManagedClubId(string userId)
        {
            var user = await _userManager.FindByIdAsync(userId);
            return user?.ManagedClubId;
        }
    }

}
