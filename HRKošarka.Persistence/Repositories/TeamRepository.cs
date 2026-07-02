using HRKošarka.Application.Contracts.Persistence;
using HRKošarka.Application.Features.Team.Queries.GetAllTeams;
using HRKošarka.Application.Features.Team.Queries.GetAvailableTeamsForPlayer;
using HRKošarka.Application.Models.Responses;
using HRKošarka.Domain;
using HRKošarka.Domain.Helpers;
using HRKošarka.Persistence.DatabaseContext;
using Microsoft.EntityFrameworkCore;

namespace HRKošarka.Persistence.Repositories
{
    public class TeamRepository : GenericRepository<Team>, ITeamRepository
    {
        public TeamRepository(HRDatabaseContext context) : base(context)
        {
        }

        public async Task<bool> IsTeamNameUniqueInClub(string name, Guid clubId, Guid ageCategoryId, Guid? excludeId = null, CancellationToken cancellationToken = default)
        {
            var query = _context.Teams.Where(x => x.Name == name && x.ClubId == clubId && x.AgeCategoryId == ageCategoryId);

            if (excludeId.HasValue)
            {
                query = query.Where(x => x.Id != excludeId.Value);
            }

            return await query.AnyAsync(cancellationToken) == false;
        }

        public async Task<PaginatedResponse<Team>> GetPagedWithIncludesAsync(GetTeamsQuery request, CancellationToken cancellationToken = default)
        {
            var query = _context.Teams
                .Include(t => t.Club)
                .Include(t => t.AgeCategory)
                .AsQueryable();

            if (request.AgeCategoryId.HasValue)
            {
                query = query.Where(t => t.AgeCategoryId == request.AgeCategoryId.Value);
            }

            if (request.Gender.HasValue)
            {
                query = query.Where(t => t.Gender == request.Gender.Value);
            }

            if (request.IsActive.HasValue)
            {
                if (request.IsActive.Value)
                {
                    query = query.Where(t => t.DeactivateDate == null);
                }
                else
                {
                    query = query.Where(t => t.DeactivateDate != null);
                }
            }


            if (!string.IsNullOrEmpty(request.SearchTerm))
            {
                var searchTerm = request.SearchTerm.ToLower();
                query = query.Where(t =>
                    t.Name.ToLower().Contains(searchTerm) ||
                    t.Club.Name.ToLower().Contains(searchTerm) ||
                    t.AgeCategory.Name.ToLower().Contains(searchTerm));
            }

            switch (request.SortBy?.ToLower())
            {
                case "name":
                    query = (request.SortDirection == "desc") ? query.OrderByDescending(t => t.Name) : query.OrderBy(t => t.Name); break;
                case "clubname":
                    query = (request.SortDirection == "desc") ? query.OrderByDescending(t => t.Club.Name) : query.OrderBy(t => t.Club.Name); break;
                case "agecategoryname":
                    query = (request.SortDirection == "desc") ? query.OrderByDescending(t => t.AgeCategory.Name) : query.OrderBy(t => t.AgeCategory.Name); break;
                default:
                    query = query.OrderBy(t => t.Name); break;
            }

            var totalCount = await query.CountAsync(cancellationToken);

            var items = await query
                .Skip((request.Page - 1) * request.PageSize)
                .Take(request.PageSize)
                .AsNoTracking()
                .ToListAsync(cancellationToken);

            return PaginatedResponse<Team>.Success(items, request.Page, request.PageSize, totalCount,
                $"Retrieved {items.Count} teams from page {request.Page}");
        }

        public async Task<Team?> GetByIdWithIncludesAsync(Guid teamId, CancellationToken cancellationToken = default)
        {
            return await _context.Teams
                .Include(t => t.Club)
                .Include(t => t.AgeCategory)
                .FirstOrDefaultAsync(t => t.Id == teamId, cancellationToken);
        }

        public async Task<Dictionary<Guid, Team>> GetByIdsAsync(IEnumerable<Guid> teamIds, CancellationToken cancellationToken = default)
        {
            return await _context.Teams
                .Where(t => teamIds.Contains(t.Id))
                .ToDictionaryAsync(t => t.Id, cancellationToken);
        }

        public async Task<List<PlayerTeamHistory>> GetTeamRosterAsync(Guid teamId, CancellationToken cancellationToken = default)
        {
            return await _context.PlayerTeamHistory
                .Include(pth => pth.Player)
                .Include(pth => pth.Season)
                .Where(pth => pth.TeamId == teamId && pth.IsActive)
                .OrderBy(pth => pth.JerseyNumber ?? 999)
                .ThenBy(pth => pth.Player.LastName)
                .ThenBy(pth => pth.Player.FirstName)
                .AsNoTracking()
                .ToListAsync(cancellationToken);
        }

        public async Task<List<AvailableTeamDTO>> GetAvailableTeamsForPlayerAsync(
            Guid playerId, string? searchTerm, CancellationToken cancellationToken = default)
        {
            var player = await _context.Players.FindAsync(new object[] { playerId }, cancellationToken);
            if (player == null) return new List<AvailableTeamDTO>();

            var playerDOB = player.DateOfBirth;
            var playerGender = player.Gender;

            var activeAssignments = await _context.PlayerTeamHistory
                .Where(pth => pth.PlayerId == playerId && pth.IsActive)
                .Select(pth => new { pth.TeamId, pth.Team.AgeCategoryId })
                .ToListAsync(cancellationToken);

            var excludedTeamIds = activeAssignments.Select(a => a.TeamId).ToList();
            var excludedAgeCategoryIds = activeAssignments.Select(a => a.AgeCategoryId).ToList();

            var query = _context.Teams
                .Include(t => t.Club)
                .Include(t => t.AgeCategory)
                .Where(t => t.DeactivateDate == null)
                .Where(t => t.Gender == playerGender)
                .Where(t => !excludedTeamIds.Contains(t.Id))
                .Where(t => !excludedAgeCategoryIds.Contains(t.AgeCategoryId))
                .AsQueryable();

            if (!string.IsNullOrEmpty(searchTerm))
            {
                var term = searchTerm.ToLower();
                query = query.Where(t =>
                    t.Name.ToLower().Contains(term) ||
                    t.Club.Name.ToLower().Contains(term) ||
                    t.AgeCategory.Name.ToLower().Contains(term));
            }

            var teams = await query
                .OrderBy(t => t.AgeCategory.Name)
                .ThenBy(t => t.Name)
                .AsNoTracking()
                .ToListAsync(cancellationToken);

            return teams
                .Where(t => AgeCategoryEligibility.IsEligible(t.AgeCategory.Code, playerDOB))
                .Select(t => new AvailableTeamDTO
                {
                    Id = t.Id,
                    Name = t.Name,
                    ClubName = t.Club.Name,
                    AgeCategoryName = t.AgeCategory.Name,
                    Gender = t.Gender
                })
                .ToList();
        }
    }
}
