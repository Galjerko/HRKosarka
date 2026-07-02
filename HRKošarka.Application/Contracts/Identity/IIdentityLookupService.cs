namespace HRKošarka.Application.Contracts.Identity
{
    public interface IIdentityLookupService
    {
        Task<string?> GetEmailByUserIdAsync(string userId, CancellationToken cancellationToken = default);
        Task<List<string>> GetUserIdsInRoleAsync(string role, CancellationToken cancellationToken = default);
    }
}
