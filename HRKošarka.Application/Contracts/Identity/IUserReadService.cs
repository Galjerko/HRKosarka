namespace HRKošarka.Application.Contracts.Identity
{
    public interface IUserReadService
    {
        Task<List<Guid>> GetManagedClubIdsAsync(CancellationToken cancellationToken = default);
    }
}
