using HRKošarka.Application.Contracts.Persistence;
using HRKošarka.Application.Features.Player.Queries.GetAvailablePlayers;
using HRKošarka.Domain;
using HRKošarka.Domain.Common;
using HRKošarka.Domain.Helpers;
using HRKošarka.Persistence.DatabaseContext;
using Microsoft.EntityFrameworkCore;

namespace HRKošarka.Persistence.Repositories
{
    public class PlayerRepository : GenericRepository<Player>, IPlayerRepository
    {
        public PlayerRepository(HRDatabaseContext context) : base(context)
        {
        }

        public async Task<bool> IsRegistrationNumberUnique(string registrationNumber, Guid? excludeId = null, CancellationToken cancellationToken = default)
        {
            var query = _context.Players.Where(p => p.RegistrationNumber == registrationNumber);

            if (excludeId.HasValue)
            {
                query = query.Where(p => p.Id != excludeId.Value);
            }

            return await query.AnyAsync(cancellationToken) == false;
        }

        public async Task<bool> IsAlreadyActiveInTeamAsync(Guid playerId, Guid teamId, CancellationToken cancellationToken = default)
        {
            return await _context.PlayerTeamHistory
                .AnyAsync(pth => pth.PlayerId == playerId && pth.TeamId == teamId && pth.IsActive, cancellationToken);
        }

        public async Task<bool> HasAgeCategoryConflictAsync(Guid playerId, Guid ageCategoryId, CancellationToken cancellationToken = default)
        {
            return await _context.PlayerTeamHistory
                .Where(pth => pth.PlayerId == playerId && pth.IsActive)
                .AnyAsync(pth => pth.Team.AgeCategoryId == ageCategoryId, cancellationToken);
        }

        public async Task<List<AvailablePlayerDTO>> GetAvailablePlayersAsync(
            Guid teamId, string? searchTerm, CancellationToken cancellationToken = default)
        {
            var team = await _context.Teams
                .Include(t => t.AgeCategory)
                .FirstOrDefaultAsync(t => t.Id == teamId, cancellationToken);
            if (team == null) return new List<AvailablePlayerDTO>();

            var targetGender = team.Gender;
            var targetAgeCategoryId = team.AgeCategoryId;
            var minBirthYear = AgeCategoryEligibility.GetMinBirthYear(team.AgeCategory.Code);

            var query = _context.Players
                .Where(p => p.DeactivateDate == null)
                .Where(p => p.Gender == targetGender)
                .Where(p => !p.TeamHistory.Any(th => th.TeamId == teamId && th.IsActive))
                .Where(p => !p.TeamHistory.Any(th => th.IsActive && th.Team.AgeCategoryId == targetAgeCategoryId))
                .AsQueryable();

            if (minBirthYear.HasValue)
                query = query.Where(p => p.DateOfBirth.Year >= minBirthYear.Value);

            if (!string.IsNullOrEmpty(searchTerm))
            {
                var term = searchTerm.ToLower();
                query = query.Where(p =>
                    p.FirstName.ToLower().Contains(term) ||
                    p.LastName.ToLower().Contains(term) ||
                    p.RegistrationNumber.ToLower().Contains(term));
            }

            return await query
                .OrderBy(p => p.LastName).ThenBy(p => p.FirstName)
                .Select(p => new AvailablePlayerDTO
                {
                    Id = p.Id,
                    FullName = p.FirstName + " " + p.LastName,
                    RegistrationNumber = p.RegistrationNumber,
                    DateOfBirth = p.DateOfBirth,
                    Position = p.Position,
                    Gender = p.Gender
                })
                .AsNoTracking()
                .ToListAsync(cancellationToken);
        }
    }
}
