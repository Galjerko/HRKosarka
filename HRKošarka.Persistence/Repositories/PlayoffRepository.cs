using HRKošarka.Application.Contracts.Persistence;
using HRKošarka.Application.Features.League.Queries.GetLeagueLeaderboard;
using HRKošarka.Application.Features.League.Queries.GetLeagueStandings;
using HRKošarka.Application.Features.League.Queries.GetPlayoffBracket;
using HRKošarka.Domain;
using HRKošarka.Domain.Common;
using HRKošarka.Persistence.DatabaseContext;
using Microsoft.EntityFrameworkCore;

namespace HRKošarka.Persistence.Repositories
{
    public class PlayoffRepository : GenericRepository<PlayoffSeries>, IPlayoffRepository
    {
        public PlayoffRepository(HRDatabaseContext context) : base(context)
        {
        }

        public async Task<PlayoffSeries?> GetSeriesWithMatchesAsync(Guid seriesId, CancellationToken ct = default)
        {
            return await _context.PlayoffSeries
                .Include(s => s.Matches)
                .Include(s => s.HomeTeam)
                .Include(s => s.AwayTeam)
                .FirstOrDefaultAsync(s => s.Id == seriesId, ct);
        }

        public async Task<PlayoffBracketDTO> GetPlayoffBracketAsync(Guid leagueId, CancellationToken ct = default)
        {
            var flatSeries = await _context.PlayoffSeries
                .Where(s => s.LeagueId == leagueId)
                .OrderBy(s => s.RoundNumber)
                // Math.Min isn't translatable to SQL — equivalent ternary form is.
                .ThenBy(s => (s.HomeSeedNumber ?? int.MaxValue) < (s.AwaySeedNumber ?? int.MaxValue)
                    ? (s.HomeSeedNumber ?? int.MaxValue)
                    : (s.AwaySeedNumber ?? int.MaxValue))
                .ThenBy(s => s.SeriesNumber)
                .Select(s => new
                {
                    s.RoundNumber,
                    s.RoundName,
                    Series = new PlayoffSeriesDTO
                    {
                        SeriesId = s.Id,
                        SeriesNumber = s.SeriesNumber,
                        HomeTeamId = s.HomeTeamId,
                        HomeTeamName = s.HomeTeam!.Name,
                        AwayTeamId = s.AwayTeamId,
                        AwayTeamName = s.AwayTeam!.Name,
                        HomeSeedNumber = s.HomeSeedNumber,
                        AwaySeedNumber = s.AwaySeedNumber,
                        WinsNeeded = s.WinsNeeded,
                        // Mirrors PlayoffSeries.HomeWins/AwayWins (normalized for venue alternation).
                        HomeWins = s.Matches.Count(m =>
                            (m.IsResultConfirmed || m.Status == MatchStatus.Forfeit)
                            && m.HomeScore.HasValue && m.AwayScore.HasValue
                            && ((m.HomeTeamId == s.HomeTeamId && m.HomeScore.Value > m.AwayScore.Value)
                             || (m.AwayTeamId == s.HomeTeamId && m.AwayScore.Value > m.HomeScore.Value))),
                        AwayWins = s.Matches.Count(m =>
                            (m.IsResultConfirmed || m.Status == MatchStatus.Forfeit)
                            && m.HomeScore.HasValue && m.AwayScore.HasValue
                            && ((m.HomeTeamId == s.AwayTeamId && m.HomeScore.Value > m.AwayScore.Value)
                             || (m.AwayTeamId == s.AwayTeamId && m.AwayScore.Value > m.HomeScore.Value))),
                        IsCompleted = s.IsCompleted,
                        WinnerId = s.WinnerId,
                        WinnerName = s.WinnerId == s.HomeTeamId ? s.HomeTeam!.Name
                            : s.WinnerId == s.AwayTeamId ? s.AwayTeam!.Name
                            : null,
                        // Match home/away alternates per game venue pattern, but scores are reported in
                        // series-Home/series-Away order so the columns stay consistent across all games.
                        Matches = s.Matches
                            .OrderBy(m => m.DefaultScheduledDate)
                            .Select(m => new PlayoffMatchSlimDTO
                            {
                                MatchId = m.Id,
                                ScheduledDate = m.ActualScheduledDate,
                                Status = m.Status,
                                HomeScore = m.HomeTeamId == s.HomeTeamId ? m.HomeScore : m.AwayScore,
                                AwayScore = m.HomeTeamId == s.HomeTeamId ? m.AwayScore : m.HomeScore,
                                IsResultConfirmed = m.IsResultConfirmed,
                                Venue = m.VenueOverride ?? m.HomeTeam!.Club!.VenueName
                            }).ToList()
                    }
                })
                .AsNoTracking()
                .ToListAsync(ct);

            // Sequence index isn't translatable to SQL, so GameNumber is filled in after materialization.
            foreach (var entry in flatSeries)
                for (int i = 0; i < entry.Series.Matches.Count; i++)
                    entry.Series.Matches[i].GameNumber = i + 1;

            var rounds = flatSeries
                .GroupBy(x => new { x.RoundNumber, x.RoundName })
                .OrderBy(g => g.Key.RoundNumber)
                .Select(g => new PlayoffRoundDTO
                {
                    RoundNumber = g.Key.RoundNumber,
                    RoundName = g.Key.RoundName,
                    Series = g.Select(x => x.Series).ToList()
                }).ToList();

            return new PlayoffBracketDTO { LeagueId = leagueId, Rounds = rounds };
        }

        public async Task<LeagueLeadersDTO> GetPlayoffLeadersAsync(Guid leagueId, CancellationToken ct = default)
        {
            var stats = await _context.PlayerMatchStats
                .Include(s => s.Player)
                .Include(s => s.Team)
                .Where(s => s.Match.LeagueId == leagueId && s.Match.PlayoffSeriesId != null
                    && !s.DidNotPlay && s.TeamId.HasValue)
                .AsNoTracking()
                .ToListAsync(ct);

            var perPlayer = stats
                .GroupBy(s => s.PlayerId)
                .Select(g => new
                {
                    PlayerId = g.Key,
                    PlayerName = $"{g.First().Player.FirstName} {g.First().Player.LastName}",
                    TeamName = g.First().Team!.Name,
                    GamesPlayed = g.Count(),
                    AveragePoints = (decimal)g.Sum(s => s.Points) / g.Count(),
                    AverageThreePointers = (decimal)g.Sum(s => s.ThreePointers) / g.Count(),
                    AverageFouls = (decimal)g.Sum(s => s.Fouls) / g.Count()
                }).ToList();

            return new LeagueLeadersDTO
            {
                TopScorers = perPlayer
                    .OrderByDescending(s => s.AveragePoints)
                    .Take(5)
                    .Select(s => new LeaderEntryDTO
                    {
                        PlayerId = s.PlayerId,
                        PlayerName = s.PlayerName,
                        TeamName = s.TeamName,
                        Value = Math.Round(s.AveragePoints, 1),
                        GamesPlayed = s.GamesPlayed
                    }).ToList(),
                TopThreePointers = perPlayer
                    .OrderByDescending(s => s.AverageThreePointers)
                    .Take(5)
                    .Select(s => new LeaderEntryDTO
                    {
                        PlayerId = s.PlayerId,
                        PlayerName = s.PlayerName,
                        TeamName = s.TeamName,
                        Value = Math.Round(s.AverageThreePointers, 1),
                        GamesPlayed = s.GamesPlayed
                    }).ToList(),
                TopFoulMakers = perPlayer
                    .OrderByDescending(s => s.AverageFouls)
                    .Take(5)
                    .Select(s => new LeaderEntryDTO
                    {
                        PlayerId = s.PlayerId,
                        PlayerName = s.PlayerName,
                        TeamName = s.TeamName,
                        Value = Math.Round(s.AverageFouls, 1),
                        GamesPlayed = s.GamesPlayed
                    }).ToList()
            };
        }

        public async Task<List<LeaguePlayerStatDTO>> GetPlayoffLeaderboardAsync(
            Guid leagueId, string? sortBy, string? sortDirection, CancellationToken ct = default)
        {
            var stats = await _context.PlayerMatchStats
                .Include(s => s.Player)
                .Include(s => s.Team)
                .Where(s => s.Match.LeagueId == leagueId && s.Match.PlayoffSeriesId != null
                    && !s.DidNotPlay && s.TeamId.HasValue)
                .AsNoTracking()
                .ToListAsync(ct);

            var list = stats
                .GroupBy(s => s.PlayerId)
                .Select(g =>
                {
                    var first = g.First();
                    var gp = g.Count();
                    var totalPoints = g.Sum(s => s.Points);
                    var totalThrees = g.Sum(s => s.ThreePointers);
                    var totalFouls = g.Sum(s => s.Fouls);

                    return new LeaguePlayerStatDTO
                    {
                        PlayerId = g.Key,
                        PlayerName = $"{first.Player.FirstName} {first.Player.LastName}",
                        PlayerPosition = first.Player.Position,
                        TeamId = first.TeamId!.Value,
                        TeamName = first.Team!.Name,
                        GamesPlayed = gp,
                        PPG = Math.Round((decimal)totalPoints / gp, 1),
                        TotalPoints = totalPoints,
                        ThreePointsPerGame = Math.Round((decimal)totalThrees / gp, 1),
                        TotalThreePoints = totalThrees,
                        FoulsPerGame = Math.Round((decimal)totalFouls / gp, 1),
                        TotalFouls = totalFouls
                    };
                })
                .ToList();

            bool asc = string.Equals(sortDirection, "asc", StringComparison.OrdinalIgnoreCase);
            return RepositorySortHelper.ApplySort(list, sortBy, asc, RepositorySortHelper.LeaguePlayerStatSortSelectors, x => x.PPG);
        }

        public async Task<List<PlayoffSeries>> GetUpcomingSeriesPopulatedByThisSeriesAsync(Guid completedSeriesId, CancellationToken ct = default)
        {
            return await _context.PlayoffSeries
                .Where(s => s.HomeFeederSeriesId == completedSeriesId || s.AwayFeederSeriesId == completedSeriesId)
                .ToListAsync(ct);
        }

        public async Task CreateInitialBracketAsync(List<PlayoffSeries> allSeries, League league, CancellationToken ct = default)
        {
            // league is already tracked, so its mutated fields are saved by CreateRangeAsync too.
            await CreateRangeAsync(allSeries, ct);
        }

        public async Task UpdateSeriesAndActivateNextAsync(
            PlayoffSeries updatedSeries,
            List<PlayoffSeries> nextRoundStubsToActivate,
            List<Match> matchesToCreate,
            CancellationToken ct = default)
        {
            // updatedSeries and nextRoundStubsToActivate are already tracked, so EF picks up their mutated fields.

            if (matchesToCreate.Count > 0)
            {
                await _context.Matches.AddRangeAsync(matchesToCreate, ct);
            }

            await _context.SaveChangesAsync(ct);
        }
    }
}
