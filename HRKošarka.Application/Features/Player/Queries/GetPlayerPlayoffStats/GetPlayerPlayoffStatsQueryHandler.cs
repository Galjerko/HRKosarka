using HRKošarka.Application.Contracts.Persistence;
using HRKošarka.Application.Features.Player.Queries.GetPlayerSeasonStats;
using HRKošarka.Application.Models.Responses;
using HRKošarka.Domain.Common;
using MediatR;

namespace HRKošarka.Application.Features.Player.Queries.GetPlayerPlayoffStats
{
    public class GetPlayerPlayoffStatsQueryHandler
        : IRequestHandler<GetPlayerPlayoffStatsQuery, QueryResponse<List<PlayerSeasonGroupDTO>>>
    {
        private readonly IPlayerMatchStatsRepository _playerMatchStatsRepository;

        public GetPlayerPlayoffStatsQueryHandler(IPlayerMatchStatsRepository playerMatchStatsRepository)
        {
            _playerMatchStatsRepository = playerMatchStatsRepository;
        }

        public async Task<QueryResponse<List<PlayerSeasonGroupDTO>>> Handle(
            GetPlayerPlayoffStatsQuery request, CancellationToken ct)
        {
            var matchStats = await _playerMatchStatsRepository.GetAllByPlayerPlayoffWithMatchAsync(request.PlayerId, ct);

            if (!matchStats.Any())
                return QueryResponse<List<PlayerSeasonGroupDTO>>.Success(new List<PlayerSeasonGroupDTO>());

            var result = matchStats
                .GroupBy(s => new { s.Match.League.SeasonId, SeasonName = s.Match.League.Season.Name })
                .OrderByDescending(g => g.Key.SeasonName)
                .Select(g =>
                {
                    var totalGP = g.Count();
                    var totalPts = g.Sum(s => s.Points);
                    var total3P = g.Sum(s => s.ThreePointers);
                    var totalF = g.Sum(s => s.Fouls);

                    var leagueStats = g
                        .GroupBy(s => s.Match.LeagueId)
                        .OrderBy(lg => lg.First().Match.League.CompetitionType)  // League(0) before Cup(1)
                        .ThenBy(lg => lg.First().Match.League.Name)
                        .Select(lg =>
                        {
                            var leagueGP = lg.Count();
                            var leagueTotalPts = lg.Sum(s => s.Points);
                            var leagueTotal3P = lg.Sum(s => s.ThreePointers);
                            var leagueTotalF = lg.Sum(s => s.Fouls);

                            var best = lg.OrderByDescending(s => s.Points).ThenByDescending(s => s.ThreePointers).First();
                            PlayerBestGameDTO? bestGame = null;
                            if (best.Points > 0)
                            {
                                var opponentName = best.TeamId == best.Match.HomeTeamId
                                    ? best.Match.AwayTeam.Name
                                    : best.Match.HomeTeam.Name;

                                bestGame = new PlayerBestGameDTO
                                {
                                    MatchId = best.MatchId,
                                    Points = best.Points,
                                    ThreePointers = best.ThreePointers,
                                    OpponentTeamName = opponentName,
                                    MatchDate = best.Match.ActualScheduledDate
                                };
                            }

                            var league = lg.First().Match.League;
                            return new PlayerLeagueStatsDTO
                            {
                                LeagueId = lg.Key,
                                LeagueName = league.Name,
                                CompetitionType = league.CompetitionType == CompetitionType.League ? "League" : "Cup",
                                TeamName = lg.First().Team!.Name,
                                GamesPlayed = leagueGP,
                                PPG = Math.Round((decimal)leagueTotalPts / leagueGP, 1),
                                ThreePG = Math.Round((decimal)leagueTotal3P / leagueGP, 1),
                                FPG = Math.Round((decimal)leagueTotalF / leagueGP, 1),
                                TotalPoints = leagueTotalPts,
                                TotalThreePointers = leagueTotal3P,
                                TotalFouls = leagueTotalF,
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
