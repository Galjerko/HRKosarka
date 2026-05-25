using HRKošarka.Application.Contracts.Persistence;
using HRKošarka.Application.Models.Responses;
using HRKošarka.Domain.Common;
using MediatR;

namespace HRKošarka.Application.Features.Player.Queries.GetPlayerSeasonStats
{
    public class GetPlayerSeasonStatsQueryHandler
        : IRequestHandler<GetPlayerSeasonStatsQuery, QueryResponse<List<PlayerSeasonGroupDTO>>>
    {
        private readonly IPlayerSeasonStatsRepository _playerSeasonStatsRepository;
        private readonly IPlayerMatchStatsRepository _playerMatchStatsRepository;

        public GetPlayerSeasonStatsQueryHandler(
            IPlayerSeasonStatsRepository playerSeasonStatsRepository,
            IPlayerMatchStatsRepository playerMatchStatsRepository)
        {
            _playerSeasonStatsRepository = playerSeasonStatsRepository;
            _playerMatchStatsRepository = playerMatchStatsRepository;
        }

        public async Task<QueryResponse<List<PlayerSeasonGroupDTO>>> Handle(
            GetPlayerSeasonStatsQuery request, CancellationToken ct)
        {
            var seasonStats = await _playerSeasonStatsRepository.GetAllByPlayerAsync(request.PlayerId, ct);

            if (!seasonStats.Any())
                return QueryResponse<List<PlayerSeasonGroupDTO>>.Success(new List<PlayerSeasonGroupDTO>());

            var matchStats = await _playerMatchStatsRepository.GetAllByPlayerWithMatchAsync(request.PlayerId, ct);

            // Best game per league: max Points, then max ThreePointers as tiebreaker
            var bestByLeague = matchStats
                .GroupBy(s => s.Match.LeagueId)
                .ToDictionary(
                    g => g.Key,
                    g => g.OrderByDescending(s => s.Points).ThenByDescending(s => s.ThreePointers).First()
                );

            var result = seasonStats
                .GroupBy(s => new { s.SeasonId, SeasonName = s.Season.Name })
                .OrderByDescending(g => g.Key.SeasonName)
                .Select(g =>
                {
                    var totalGP = g.Sum(s => s.MatchesPlayed);
                    var totalPts = g.Sum(s => s.TotalPoints);
                    var total3P = g.Sum(s => s.TotalThreePointers);
                    var totalF = g.Sum(s => s.TotalFouls);

                    var leagueStats = g
                        .OrderBy(s => s.League.CompetitionType)  // League(0) before Cup(1)
                        .ThenBy(s => s.League.Name)
                        .Select(s =>
                        {
                            PlayerBestGameDTO? bestGame = null;
                            if (bestByLeague.TryGetValue(s.LeagueId, out var best) && best.Points > 0)
                            {
                                var opponentName = best.TeamId == best.Match.HomeTeamId
                                    ? best.Match.AwayTeam.Name
                                    : best.Match.HomeTeam.Name;

                                bestGame = new PlayerBestGameDTO
                                {
                                    Points = best.Points,
                                    ThreePointers = best.ThreePointers,
                                    OpponentTeamName = opponentName,
                                    MatchDate = best.Match.ActualScheduledDate
                                };
                            }

                            return new PlayerLeagueStatsDTO
                            {
                                LeagueId = s.LeagueId,
                                LeagueName = s.League.Name,
                                CompetitionType = s.League.CompetitionType == CompetitionType.League ? "League" : "Cup",
                                TeamName = s.Team.Name,
                                GamesPlayed = s.MatchesPlayed,
                                PPG = s.AveragePoints,
                                ThreePG = s.AverageThreePointers,
                                FPG = s.AverageFouls,
                                TotalPoints = s.TotalPoints,
                                TotalThreePointers = s.TotalThreePointers,
                                TotalFouls = s.TotalFouls,
                                BestGame = bestGame
                            };
                        }).ToList();

                    return new PlayerSeasonGroupDTO
                    {
                        SeasonId = g.Key.SeasonId,
                        SeasonName = g.Key.SeasonName,
                        TotalGamesPlayed = totalGP,
                        CombinedPPG = totalGP > 0 ? Math.Round((decimal)totalPts / totalGP, 1) : 0,
                        Combined3PG = totalGP > 0 ? Math.Round((decimal)total3P / totalGP, 1) : 0,
                        CombinedFPG = totalGP > 0 ? Math.Round((decimal)totalF / totalGP, 1) : 0,
                        LeagueStats = leagueStats
                    };
                }).ToList();

            return QueryResponse<List<PlayerSeasonGroupDTO>>.Success(result);
        }
    }
}
