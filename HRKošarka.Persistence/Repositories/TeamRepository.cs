using HRKošarka.Application.Contracts.Persistence;
using HRKošarka.Application.Features.Team.Queries.GetAllTeams;
using HRKošarka.Application.Models.Responses;
using HRKošarka.Domain;
using HRKošarka.Persistence.DatabaseContext;
using Microsoft.EntityFrameworkCore;

namespace HRKošarka.Persistence.Repositories
{
    public class TeamRepository : GenericRepository<Team>, ITeamRepository
    {
        public TeamRepository(HRDatabaseContext context) : base(context)
        {
        }

        public async Task<bool> IsTeamNameUniqueInClub(string name, Guid clubId, Guid ageCategoryId, Guid? excludeId = null)
        {
            var query = _context.Teams.Where(x => x.Name == name && x.ClubId == clubId && x.AgeCategoryId == ageCategoryId);

            if (excludeId.HasValue)
            {
                query = query.Where(x => x.Id != excludeId.Value);
            }

            return await query.AnyAsync() == false;
        }

        public async Task<PaginatedResponse<Team>> GetPagedWithIncludesAsync(GetTeamsQuery request)
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

            var totalCount = await query.CountAsync();

            var items = await query
                .Skip((request.Page - 1) * request.PageSize)
                .Take(request.PageSize)
                .AsNoTracking()
                .ToListAsync();

            return PaginatedResponse<Team>.Success(items, request.Page, request.PageSize, totalCount,
                $"Retrieved {items.Count} teams from page {request.Page}");
        }

        public async Task<Team?> GetByIdWithIncludesAsync(Guid teamId)
        {
            return await _context.Teams
                .Include(t => t.Club)
                .Include(t => t.AgeCategory)
                .FirstOrDefaultAsync(t => t.Id == teamId);
        }
    }
}
