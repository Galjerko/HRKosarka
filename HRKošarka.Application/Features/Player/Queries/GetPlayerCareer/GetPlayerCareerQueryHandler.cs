using HRKošarka.Application.Contracts.Persistence;
using HRKošarka.Application.Models.Responses;
using HRKošarka.Domain.Common;
using MediatR;

namespace HRKošarka.Application.Features.Player.Queries.GetPlayerCareer
{
    public class GetPlayerCareerQueryHandler
        : IRequestHandler<GetPlayerCareerQuery, QueryResponse<List<PlayerCareerItemDTO>>>
    {
        private readonly IPlayerTeamHistoryRepository _historyRepository;
        private readonly IPlayerSeasonStatsRepository _statsRepository;
        private readonly IPlayerMatchStatsRepository _matchStatsRepository;

        public GetPlayerCareerQueryHandler(
            IPlayerTeamHistoryRepository historyRepository,
            IPlayerSeasonStatsRepository statsRepository,
            IPlayerMatchStatsRepository matchStatsRepository)
        {
            _historyRepository = historyRepository;
            _statsRepository = statsRepository;
            _matchStatsRepository = matchStatsRepository;
        }

        public async Task<QueryResponse<List<PlayerCareerItemDTO>>> Handle(
            GetPlayerCareerQuery request, CancellationToken ct)
        {
            var history = await _historyRepository.GetAllByPlayerAsync(request.PlayerId, ct);
            var allStats = await _statsRepository.GetAllByPlayerAsync(request.PlayerId, ct);
            var playoffMatchStats = await _matchStatsRepository.GetAllByPlayerPlayoffWithMatchAsync(request.PlayerId, ct);

            var statsByTeamSeason = allStats
                .GroupBy(s => (s.TeamId, s.SeasonId))
                .ToDictionary(
                    g => g.Key,
                    g => g
                        .Select(s => new PlayerCareerLeagueStatDTO
                        {
                            LeagueId = s.LeagueId,
                            LeagueName = s.League.Name,
                            CompetitionType = s.League.CompetitionType == CompetitionType.League ? "League" : "Cup",
                            GamesPlayed = s.MatchesPlayed,
                            PPG = s.AveragePoints,
                            ThreePG = s.AverageThreePointers,
                            FPG = s.AverageFouls
                        }).ToList());

            // Playoff games never write to PlayerSeasonStats, so aggregate them from PlayerMatchStats directly
            var playoffStatsByTeamSeason = playoffMatchStats
                .GroupBy(s => (s.TeamId!.Value, s.Match.League.SeasonId))
                .ToDictionary(
                    g => g.Key,
                    g => g.GroupBy(s => s.Match.LeagueId)
                        .Select(lg =>
                        {
                            var gamesPlayed = lg.Count();
                            return new PlayerCareerLeagueStatDTO
                            {
                                LeagueId = lg.Key,
                                LeagueName = lg.First().Match.League.Name,
                                CompetitionType = "Playoffs",
                                IsPlayoff = true,
                                GamesPlayed = gamesPlayed,
                                PPG = Math.Round((decimal)lg.Sum(s => s.Points) / gamesPlayed, 1),
                                ThreePG = Math.Round((decimal)lg.Sum(s => s.ThreePointers) / gamesPlayed, 1),
                                FPG = Math.Round((decimal)lg.Sum(s => s.Fouls) / gamesPlayed, 1)
                            };
                        }).ToList());

            var data = history
                .OrderByDescending(h => h.JoinDate)
                .Select(h =>
                {
                    var key = (h.TeamId, h.SeasonId);
                    statsByTeamSeason.TryGetValue(key, out var regularStats);
                    playoffStatsByTeamSeason.TryGetValue(key, out var playoffStats);

                    var compStats = (regularStats ?? new List<PlayerCareerLeagueStatDTO>())
                        .Concat(playoffStats ?? new List<PlayerCareerLeagueStatDTO>())
                        .OrderBy(s => s.CompetitionType == "Cup" ? 1 : 0)
                        .ThenBy(s => s.LeagueName)
                        .ThenBy(s => s.IsPlayoff)
                        .ToList();

                    return new PlayerCareerItemDTO
                    {
                        Id = h.Id,
                        TeamId = h.TeamId,
                        TeamName = h.Team.Name,
                        ClubName = h.Team.Club.Name,
                        SeasonName = h.Season.Name,
                        JerseyNumber = h.JerseyNumber,
                        JoinDate = h.JoinDate,
                        LeaveDate = h.LeaveDate,
                        IsActive = h.IsActive,
                        CompetitionStats = compStats
                    };
                }).ToList();

            return QueryResponse<List<PlayerCareerItemDTO>>.Success(data);
        }
    }
}
