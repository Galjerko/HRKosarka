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

        public GetPlayerCareerQueryHandler(
            IPlayerTeamHistoryRepository historyRepository,
            IPlayerSeasonStatsRepository statsRepository)
        {
            _historyRepository = historyRepository;
            _statsRepository = statsRepository;
        }

        public async Task<QueryResponse<List<PlayerCareerItemDTO>>> Handle(
            GetPlayerCareerQuery request, CancellationToken ct)
        {
            var history = await _historyRepository.GetAllByPlayerAsync(request.PlayerId, ct);
            var allStats = await _statsRepository.GetAllByPlayerAsync(request.PlayerId, ct);

            // Per-competition rows grouped by (TeamId, SeasonId)
            var statsByTeamSeason = allStats
                .GroupBy(s => (s.TeamId, s.SeasonId))
                .ToDictionary(
                    g => g.Key,
                    g => g
                        .OrderBy(s => s.League.CompetitionType)   // League(0) before Cup(1)
                        .ThenBy(s => s.League.Name)
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

            var data = history
                .OrderByDescending(h => h.JoinDate)
                .Select(h =>
                {
                    statsByTeamSeason.TryGetValue((h.TeamId, h.SeasonId), out var compStats);
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
                        CompetitionStats = compStats ?? new List<PlayerCareerLeagueStatDTO>()
                    };
                }).ToList();

            return QueryResponse<List<PlayerCareerItemDTO>>.Success(data);
        }
    }
}
