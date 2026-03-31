using HRKošarka.Application.Contracts.Persistence;
using HRKošarka.Application.Features.League.Queries.GetAllLeagues;
using HRKošarka.Application.Models.Responses;
using HRKošarka.Domain;
using HRKošarka.Persistence.DatabaseContext;
using Microsoft.EntityFrameworkCore;

namespace HRKošarka.Persistence.Repositories
{
    public class LeagueRepository : GenericRepository<League>, ILeagueRepository
    {
        public LeagueRepository(HRDatabaseContext context) : base(context)
        {
        }

        public async Task<League?> GetByIdWithIncludesAsync(Guid id, CancellationToken cancellationToken = default)
        {
            return await _context.Leagues
                .Include(l => l.Season)
                .Include(l => l.AgeCategory)
                .AsNoTracking()
                .FirstOrDefaultAsync(l => l.Id == id, cancellationToken);
        }

        public async Task<League?> GetLeagueWithDetailsAsync(Guid id, CancellationToken cancellationToken = default)
        {
            return await _context.Leagues
                .Include(l => l.Season)
                .Include(l => l.AgeCategory)
                .FirstOrDefaultAsync(l => l.Id == id, cancellationToken);
        }

        public async Task<PaginatedResponse<League>> GetPagedWithIncludesAsync(
            GetLeaguesQuery request, CancellationToken cancellationToken = default)
        {
            var query = _context.Leagues
                .Include(l => l.Season)
                .Include(l => l.AgeCategory)
                .AsQueryable();

            if (request.SeasonId.HasValue)
            {
                query = query.Where(l => l.SeasonId == request.SeasonId.Value);
            }

            if (request.AgeCategoryId.HasValue)
            {
                query = query.Where(l => l.AgeCategoryId == request.AgeCategoryId.Value);
            }

            if (request.Gender.HasValue)
            {
                query = query.Where(l => l.Gender == request.Gender.Value);
            }

            if (request.CompetitionType.HasValue)
            {
                query = query.Where(l => l.CompetitionType == request.CompetitionType.Value);
            }

            if (request.IsActive.HasValue)
            {
                if (request.IsActive.Value)
                {
                    query = query.Where(l => l.DeactivateDate == null);
                }
                else
                {
                    query = query.Where(l => l.DeactivateDate != null);
                }
            }

            if (!string.IsNullOrWhiteSpace(request.SearchTerm))
            {
                var term = request.SearchTerm.Trim().ToLower();
                query = query.Where(l => l.Name.ToLower().Contains(term));
            }

            switch (request.SortBy?.ToLower())
            {
                case "startdate":
                    query = request.SortDirection == "desc"
                        ? query.OrderByDescending(l => l.StartDate)
                        : query.OrderBy(l => l.StartDate);
                    break;
                case "enddate":
                    query = request.SortDirection == "desc"
                        ? query.OrderByDescending(l => l.EndDate)
                        : query.OrderBy(l => l.EndDate);
                    break;
                case "datecreated":
                    query = request.SortDirection == "desc"
                        ? query.OrderByDescending(l => l.DateCreated)
                        : query.OrderBy(l => l.DateCreated);
                    break;
                default:
                    query = request.SortDirection == "desc"
                        ? query.OrderByDescending(l => l.Name)
                        : query.OrderBy(l => l.Name);
                    break;
            }

            var totalCount = await query.CountAsync(cancellationToken);

            var items = await query
                .Skip((request.Page - 1) * request.PageSize)
                .Take(request.PageSize)
                .AsNoTracking()
                .ToListAsync(cancellationToken);

            return PaginatedResponse<League>.Success(
                items,
                request.Page,
                request.PageSize,
                totalCount,
                $"Retrieved {items.Count} leagues from page {request.Page}"
            );
        }
    }
}
