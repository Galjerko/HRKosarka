using HRKošarka.Application.Contracts.Identity;
using HRKošarka.Application.Models.Identity;
using HRKošarka.Identity.Models;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;

namespace HRKošarka.Identity.Services
{
    public class UserService : IUserService
    {
        private readonly UserManager<ApplicationUser> _userManager;

        public UserService(UserManager<ApplicationUser> userManager)
        {
            _userManager = userManager;
        }

        public async Task<User> GetUser(string userId)
        {
            var user = await _userManager.FindByIdAsync(userId);
            return new User
            {
                Email = user.Email,
                FirstName = user.FirstName,
                LastName = user.LastName,
                UserName = user.UserName,
                Id = user.Id,
            };
        }

        public async Task<List<User>> GetUsers()
        {
            var users = await _userManager.Users.ToListAsync();
            return users.Select(u => new User
            {
                Email = u.Email,
                FirstName = u.FirstName,
                LastName = u.LastName,
                UserName = u.UserName,
                Id = u.Id,
            }).ToList();
        }

        public async Task<List<User>> GetTeamRepresentatives()
        {
            var teamReps = await _userManager.GetUsersInRoleAsync("TeamRepresentative");
            return teamReps.Select(u => new User
            {
                Email = u.Email,
                FirstName = u.FirstName,
                LastName = u.LastName,
                UserName = u.UserName,
                Id = u.Id,
            }).ToList();
        }

        public async Task<List<User>> GetClubManagers()
        {
            var clubManagers = await _userManager.GetUsersInRoleAsync("ClubManager");
            return clubManagers.Select(u => new User
            {
                Email = u.Email,
                FirstName = u.FirstName,
                LastName = u.LastName,
                UserName = u.UserName,
                Id = u.Id,
            }).ToList();
        }
    }

}
