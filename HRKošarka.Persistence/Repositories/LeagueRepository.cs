using HRKošarka.Application.Contracts.Persistence;
using HRKošarka.Application.Features.League.Queries.GetAllLeagues;
using HRKošarka.Application.Features.League.Queries.GetAvailableTeamsForLeague;
using HRKošarka.Application.Features.League.Queries.GetLeagueTeams;
using HRKošarka.Application.Features.Team.Queries.GetTeamLeagues;
using HRKošarka.Application.Models.Responses;
using HRKošarka.Domain;
using HRKošarka.Domain.Common;
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

        public async Task<List<LeagueTeamDTO>> GetLeagueTeamsAsync(Guid leagueId, CancellationToken cancellationToken = default)
        {
            return await _context.LeagueTeams
                .Include(lt => lt.Team).ThenInclude(t => t.Club)
                .Include(lt => lt.Team).ThenInclude(t => t.AgeCategory)
                .Where(lt => lt.LeagueId == leagueId && lt.IsActive && lt.Team.DateDeleted == null)
                .OrderBy(lt => lt.Team.Name)
                .Select(lt => new LeagueTeamDTO
                {
                    Id = lt.Id,
                    TeamId = lt.TeamId,
                    TeamName = lt.Team.Name,
                    ClubName = lt.Team.Club.Name,
                    AgeCategoryName = lt.Team.AgeCategory.Name,
                    RegistrationDate = lt.RegistrationDate
                })
                .AsNoTracking()
                .ToListAsync(cancellationToken);
        }

        public async Task<List<AvailableTeamForLeagueDTO>> GetAvailableTeamsForLeagueAsync(
            Guid leagueId, string? searchTerm, CancellationToken cancellationToken = default)
        {
            var league = await _context.Leagues
                .AsNoTracking()
                .FirstOrDefaultAsync(l => l.Id == leagueId, cancellationToken);
            if (league == null) return new List<AvailableTeamForLeagueDTO>();

            var registeredTeamIds = await _context.LeagueTeams
                .Where(lt => lt.LeagueId == leagueId && lt.IsActive)
                .Select(lt => lt.TeamId)
                .ToListAsync(cancellationToken);

            var query = _context.Teams
                .Include(t => t.Club)
                .Include(t => t.AgeCategory)
                .Where(t => t.DeactivateDate == null && t.DateDeleted == null)
                .Where(t => t.Gender == league.Gender)
                .Where(t => t.AgeCategoryId == league.AgeCategoryId)
                .Where(t => !registeredTeamIds.Contains(t.Id));

            if (!string.IsNullOrWhiteSpace(searchTerm))
            {
                var term = searchTerm.Trim().ToLower();
                query = query.Where(t => t.Name.ToLower().Contains(term) || t.Club.Name.ToLower().Contains(term));
            }

            return await query
                .OrderBy(t => t.Name)
                .Select(t => new AvailableTeamForLeagueDTO
                {
                    Id = t.Id,
                    Name = t.Name,
                    ClubName = t.Club.Name,
                    AgeCategoryName = t.AgeCategory.Name
                })
                .AsNoTracking()
                .ToListAsync(cancellationToken);
        }

        public async Task<LeagueTeam?> GetLeagueTeamAsync(Guid leagueId, Guid teamId, CancellationToken cancellationToken = default)
        {
            return await _context.LeagueTeams
                .FirstOrDefaultAsync(lt => lt.LeagueId == leagueId && lt.TeamId == teamId, cancellationToken);
        }

        public async Task<List<TeamLeagueDTO>> GetTeamLeaguesAsync(Guid teamId, CancellationToken cancellationToken = default)
        {
            return await _context.LeagueTeams
                .Include(lt => lt.League).ThenInclude(l => l.Season)
                .Include(lt => lt.League).ThenInclude(l => l.AgeCategory)
                .Where(lt => lt.TeamId == teamId && lt.IsActive && lt.League.DateDeleted == null)
                .OrderByDescending(lt => lt.League.StartDate)
                .Select(lt => new TeamLeagueDTO
                {
                    LeagueId = lt.LeagueId,
                    LeagueName = lt.League.Name,
                    SeasonName = lt.League.Season.Name,
                    AgeCategoryName = lt.League.AgeCategory.Name,
                    Gender = lt.League.Gender,
                    CompetitionType = lt.League.CompetitionType,
                    RegistrationDate = lt.RegistrationDate,
                    IsLeagueActive = lt.League.DeactivateDate == null
                })
                .AsNoTracking()
                .ToListAsync(cancellationToken);
        }

        public async Task DeactivateAllForTeamAsync(Guid teamId, CancellationToken cancellationToken = default)
        {
            var registrations = await _context.LeagueTeams
                .Where(lt => lt.TeamId == teamId && lt.IsActive)
                .ToListAsync(cancellationToken);

            foreach (var lt in registrations)
                lt.IsActive = false;

            await _context.SaveChangesAsync(cancellationToken);
        }

        public async Task DeactivateAllForLeagueAsync(Guid leagueId, CancellationToken cancellationToken = default)
        {
            var registrations = await _context.LeagueTeams
                .Where(lt => lt.LeagueId == leagueId && lt.IsActive)
                .ToListAsync(cancellationToken);

            foreach (var lt in registrations)
                lt.IsActive = false;

            await _context.SaveChangesAsync(cancellationToken);
        }
    }
}
