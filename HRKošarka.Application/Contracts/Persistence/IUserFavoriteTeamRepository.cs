using HRKošarka.Application.Features.UserFavoriteTeam.Queries.GetMyFavoriteTeams;
using HRKošarka.Domain;

namespace HRKošarka.Application.Contracts.Persistence
{
    public interface IUserFavoriteTeamRepository : IGenericRepository<UserFavoriteTeam>
    {
        Task<UserFavoriteTeam?> GetByUserAndTeamAsync(string userId, Guid teamId, CancellationToken ct = default);
        Task<bool> IsFavoritedAsync(string userId, Guid teamId, CancellationToken ct = default);
        Task<List<FavoriteTeamDTO>> GetByUserAsync(string userId, CancellationToken ct = default);
        Task<List<string>> GetUserIdsByTeamAsync(Guid teamId, CancellationToken ct = default);
    }
}
