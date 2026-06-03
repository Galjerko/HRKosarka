using HRKošarka.Application.Contracts.Persistence;
using HRKošarka.Application.Features.League.Queries.GetAllLeagues;
using HRKošarka.Application.Features.League.Queries.GetAvailableTeamsForLeague;
using HRKošarka.Application.Features.League.Queries.GetFeaturedLeagueMatches;
using HRKošarka.Application.Features.League.Queries.GetLeagueBreaks;
using HRKošarka.Application.Features.League.Queries.GetLeagueSchedule;
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
                    ClubId = lt.Team.ClubId,
                    ClubName = lt.Team.Club.Name,
                    ClubImageBytes = lt.Team.Club.ImageBytes,
                    ClubImageContentType = lt.Team.Club.ImageContentType,
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
                .Where(t => t.DeactivateDate == null)
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

        public async Task<List<LeagueBreakDTO>> GetLeagueBreaksAsync(Guid leagueId, CancellationToken cancellationToken = default)
        {
            return await _context.LeagueBreaks
                .Where(b => b.LeagueId == leagueId)
                .OrderBy(b => b.StartDate)
                .Select(b => new LeagueBreakDTO
                {
                    Id = b.Id,
                    Name = b.Name,
                    StartDate = b.StartDate,
                    EndDate = b.EndDate
                })
                .AsNoTracking()
                .ToListAsync(cancellationToken);
        }

        public async Task<List<LeagueRoundDTO>> GetLeagueScheduleAsync(Guid leagueId, CancellationToken cancellationToken = default)
        {
            var matches = await _context.Matches
                .Include(m => m.HomeTeam)
                .Include(m => m.AwayTeam)
                .Where(m => m.LeagueId == leagueId
                         && m.HomeTeam.DateDeleted == null
                         && m.AwayTeam.DateDeleted == null)
                .OrderBy(m => m.Round)
                .ThenBy(m => m.HomeTeam.Name)
                .Select(m => new
                {
                    m.Id,
                    m.Round,
                    m.RoundName,
                    m.DefaultScheduledDate,
                    m.ActualScheduledDate,
                    m.Status,
                    m.HomeScore,
                    m.AwayScore,
                    HomeTeamId = m.HomeTeamId,
                    HomeTeamName = m.HomeTeam.Name,
                    AwayTeamId = m.AwayTeamId,
                    AwayTeamName = m.AwayTeam.Name,
                    Venue = m.VenueOverride ?? m.HomeTeam.Club.VenueName
                })
                .AsNoTracking()
                .ToListAsync(cancellationToken);

            return matches
                .GroupBy(m => m.Round)
                .Select(g => new LeagueRoundDTO
                {
                    Round = g.Key,
                    RoundName = g.First().RoundName ?? $"Round {g.Key}",
                    ScheduledDate = g.First().DefaultScheduledDate,
                    Matches = g.Select(m => new LeagueMatchDTO
                    {
                        Id = m.Id,
                        HomeTeamId = m.HomeTeamId,
                        HomeTeamName = m.HomeTeamName,
                        AwayTeamId = m.AwayTeamId,
                        AwayTeamName = m.AwayTeamName,
                        DefaultScheduledDate = m.DefaultScheduledDate,
                        ActualScheduledDate = m.ActualScheduledDate,
                        Status = m.Status,
                        HomeScore = m.HomeScore,
                        AwayScore = m.AwayScore,
                        Venue = m.Venue
                    }).ToList()
                })
                .ToList();
        }

        public async Task<bool> HasActiveMatchesForTeamAsync(Guid teamId, CancellationToken cancellationToken = default)
        {
            return await _context.Matches
                .AnyAsync(m => (m.HomeTeamId == teamId || m.AwayTeamId == teamId)
                            && m.Status != MatchStatus.Completed
                            && m.Status != MatchStatus.Forfeit,
                          cancellationToken);
        }

        public async Task<List<FeaturedLeagueRoundDTO>> GetFeaturedLeagueMatchesAsync(CancellationToken cancellationToken = default)
        {
            var now = DateTime.Now;

            var featuredLeagues = await _context.Leagues
                .Where(l => l.IsFeatured && l.ScheduleGenerated)
                .AsNoTracking()
                .ToListAsync(cancellationToken);

            var result = new List<FeaturedLeagueRoundDTO>();

            foreach (var league in featuredLeagues)
            {
                // Summarise rounds without loading full match data
                var roundSummaries = await _context.Matches
                    .Where(m => m.LeagueId == league.Id)
                    .GroupBy(m => m.Round)
                    .Select(g => new
                    {
                        Round = g.Key,
                        RoundName = g.Min(m => m.RoundName),
                        AllCompleted = !g.Any(m => m.Status != MatchStatus.Completed),
                        AnyStarted = g.Any(m => m.ActualScheduledDate <= now)
                    })
                    .OrderBy(x => x.Round)
                    .AsNoTracking()
                    .ToListAsync(cancellationToken);

                if (!roundSummaries.Any()) continue;

                // Active round = first round that is not fully completed
                var activeRound = roundSummaries.FirstOrDefault(r => !r.AllCompleted)
                                  ?? roundSummaries.Last();

                // If active round hasn't started yet, fall back to the previous round
                int displayRound = activeRound.Round;
                if (!activeRound.AnyStarted && activeRound.Round > roundSummaries[0].Round)
                {
                    var prev = roundSummaries.LastOrDefault(r => r.Round < activeRound.Round);
                    if (prev != null) displayRound = prev.Round;
                }

                var displayRoundName = roundSummaries
                    .FirstOrDefault(r => r.Round == displayRound)?.RoundName
                    ?? $"Round {displayRound}";

                // Get the scheduled date of the display round so we can pull in
                // all rounds on the same day (e.g. Final + 3rd Place in a cup).
                var displayDate = await _context.Matches
                    .Where(m => m.LeagueId == league.Id && m.Round == displayRound)
                    .Select(m => (DateTime?)m.DefaultScheduledDate)
                    .FirstOrDefaultAsync(cancellationToken);

                var matches = await _context.Matches
                    .Where(m => m.LeagueId == league.Id
                             && (m.Round == displayRound
                                 || (displayDate.HasValue
                                     && m.DefaultScheduledDate.Date == displayDate.Value.Date)))
                    .OrderBy(m => m.Round)
                    .ThenBy(m => m.ActualScheduledDate)
                    .Select(m => new FeaturedMatchDTO
                    {
                        Id = m.Id,
                        HomeTeamId = m.HomeTeamId,
                        HomeTeamName = m.HomeTeam.Name,
                        HomeTeamLogoBytes = m.HomeTeam.Club.ImageBytes,
                        HomeTeamLogoContentType = m.HomeTeam.Club.ImageContentType,
                        AwayTeamId = m.AwayTeamId,
                        AwayTeamName = m.AwayTeam.Name,
                        AwayTeamLogoBytes = m.AwayTeam.Club.ImageBytes,
                        AwayTeamLogoContentType = m.AwayTeam.Club.ImageContentType,
                        ActualScheduledDate = m.ActualScheduledDate,
                        Status = m.Status,
                        HomeScore = m.HomeScore,
                        AwayScore = m.AwayScore
                    })
                    .AsNoTracking()
                    .ToListAsync(cancellationToken);

                result.Add(new FeaturedLeagueRoundDTO
                {
                    LeagueId = league.Id,
                    LeagueName = league.Name,
                    LeagueImageBytes = league.ImageBytes,
                    LeagueImageContentType = league.ImageContentType,
                    RoundNumber = displayRound,
                    RoundName = displayRoundName ?? $"Round {displayRound}",
                    Matches = matches
                });
            }

            return result
                .OrderBy(r => r.Matches.FirstOrDefault()?.ActualScheduledDate ?? DateTime.MaxValue)
                .ToList();
        }
    }
}
