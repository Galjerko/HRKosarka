using HRKošarka.Application.Models.Identity;

namespace HRKošarka.Application.Contracts.Identity
{
    public interface IClubManagerService
    {
        Task<ClubManagerResult> AssignClubManager(string userId, Guid clubId);
        Task<ClubManagerResult> RemoveClubManager(string userId);
        Task<Guid?> GetManagedClubId(string userId);
    }
}
