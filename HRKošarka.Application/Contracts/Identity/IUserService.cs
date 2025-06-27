using HRKošarka.Application.Models.Identity;

namespace HRKošarka.Application.Contracts.Identity
{
    public interface IUserService
    {
        Task<User> GetUser(string userId);
        Task<List<User>> GetUsers();
        Task<List<User>> GetTeamRepresentatives();
    }
}
