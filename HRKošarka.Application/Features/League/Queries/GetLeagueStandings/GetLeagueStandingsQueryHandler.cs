using HRKošarka.Application.Contracts.Persistence;
using HRKošarka.Application.Models.Responses;
using MediatR;

namespace HRKošarka.Application.Features.League.Queries.GetLeagueStandings
{
    public class GetLeagueStandingsQueryHandler
        : IRequestHandler<GetLeagueStandingsQuery, QueryResponse<LeagueStandingsDTO>>
    {
        private readonly ILeagueRepository _leagueRepository;
        private readonly ILeagueStandingRepository _standingRepository;
        private readonly IMatchRepository _matchRepository;
        private readonly IPlayerSeasonStatsRepository _playerStatsRepository;

        public GetLeagueStandingsQueryHandler(
            ILeagueRepository leagueRepository,
            ILeagueStandingRepository standingRepository,
            IMatchRepository matchRepository,
            IPlayerSeasonStatsRepository playerStatsRepository)
        {
            _leagueRepository = leagueRepository;
            _standingRepository = standingRepository;
            _matchRepository = matchRepository;
            _playerStatsRepository = playerStatsRepository;
        }

        public async Task<QueryResponse<LeagueStandingsDTO>> Handle(
            GetLeagueStandingsQuery request, CancellationToken ct)
        {
            var leagueTeams = await _leagueRepository.GetLeagueTeamsAsync(request.LeagueId, ct);
            var standingRecords = await _standingRepository.GetByLeagueAsync(request.LeagueId, ct);
            var completedMatches = await _matchRepository.GetCompletedMatchesByLeagueAsync(request.LeagueId, ct);
            var leaders = await _playerStatsRepository.GetLeagueLeadersAsync(request.LeagueId, ct);

            var rows = leagueTeams.Select(lt =>
            {
                var s = standingRecords.FirstOrDefault(x => x.TeamId == lt.TeamId);
                var teamMatches = completedMatches
                    .Where(m => m.HomeTeamId == lt.TeamId || m.AwayTeamId == lt.TeamId)
                    .Where(m => m.HomeScore.HasValue && m.AwayScore.HasValue)
                    .Take(5)
                    .Select(m =>
                    {
                        bool isHome = m.HomeTeamId == lt.TeamId;
                        bool won = isHome ? m.HomeScore > m.AwayScore : m.AwayScore > m.HomeScore;
                        return won ? "W" : "L";
                    })
                    .ToList();

                return new TeamStandingRowDTO
                {
                    TeamId = lt.TeamId,
                    TeamName = lt.TeamName,
                    ClubName = lt.ClubName,
                    GamesPlayed = s?.MatchesPlayed ?? 0,
                    Wins = s?.Wins ?? 0,
                    Losses = s?.Losses ?? 0,
                    PointsFor = s?.PointsFor ?? 0,
                    PointsAgainst = s?.PointsAgainst ?? 0,
                    PointsDifference = s?.PointsDifference ?? 0,
                    LeaguePoints = s?.LeaguePoints ?? 0,
                    Last5 = teamMatches,
                    HasPlayed = s != null
                };
            }).ToList();

            rows = rows
                .OrderByDescending(r => r.LeaguePoints)
                .ThenByDescending(r => r.PointsDifference)
                .ThenByDescending(r => r.PointsFor)
                .ThenBy(r => r.TeamName)
                .ToList();

            for (int i = 0; i < rows.Count; i++)
                rows[i].Position = i + 1;

            return QueryResponse<LeagueStandingsDTO>.Success(new LeagueStandingsDTO
            {
                Standings = rows,
                Leaders = leaders.TopScorers.Any() ? leaders : null
            });
        }
    }
}
