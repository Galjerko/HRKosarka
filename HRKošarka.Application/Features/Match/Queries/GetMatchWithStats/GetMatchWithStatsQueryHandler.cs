using HRKošarka.Application.Contracts.Persistence;
using HRKošarka.Application.Exceptions;
using HRKošarka.Application.Models.Responses;
using HRKošarka.Domain;
using MediatR;

namespace HRKošarka.Application.Features.Match.Queries.GetMatchWithStats
{
    public class GetMatchWithStatsQueryHandler : IRequestHandler<GetMatchWithStatsQuery, QueryResponse<MatchWithStatsDTO>>
    {
        private readonly IMatchRepository _matchRepository;
        private readonly IPlayerTeamHistoryRepository _historyRepository;
        private readonly IMatchReschedulingRequestRepository _reschedulingRepository;
        private readonly ILeagueRepository _leagueRepository;

        public GetMatchWithStatsQueryHandler(
            IMatchRepository matchRepository,
            IPlayerTeamHistoryRepository historyRepository,
            IMatchReschedulingRequestRepository reschedulingRepository,
            ILeagueRepository leagueRepository)
        {
            _matchRepository = matchRepository;
            _historyRepository = historyRepository;
            _reschedulingRepository = reschedulingRepository;
            _leagueRepository = leagueRepository;
        }

        public async Task<QueryResponse<MatchWithStatsDTO>> Handle(GetMatchWithStatsQuery request, CancellationToken ct)
        {
            var match = await _matchRepository.GetMatchWithFullDetailsAsync(request.Id, ct)
                ?? throw new NotFoundException("Match", request.Id);

            var seasonId = match.League.SeasonId;

            var homeRoster = await _historyRepository.GetRosterAsync(match.HomeTeamId, seasonId, ct);
            var awayRoster = await _historyRepository.GetRosterAsync(match.AwayTeamId, seasonId, ct);

            var existingStats = (match.PlayerStats ?? Enumerable.Empty<Domain.PlayerMatchStats>())
                .ToDictionary(s => s.PlayerId);

            var pendingReschedule = await _reschedulingRepository.GetActiveForMatchAsync(match.Id, ct);
            var leagueBreaks = await _leagueRepository.GetLeagueBreaksAsync(match.LeagueId, ct);

            var dto = new MatchWithStatsDTO
            {
                Id = match.Id,
                LeagueId = match.LeagueId,
                LeagueName = match.League.Name,
                Round = match.Round,
                RoundName = match.RoundName ?? $"Round {match.Round}",
                HomeTeamId = match.HomeTeamId,
                HomeTeamName = match.HomeTeam.Name,
                HomeTeamClubId = match.HomeTeam.ClubId,
                AwayTeamId = match.AwayTeamId,
                AwayTeamName = match.AwayTeam.Name,
                AwayTeamClubId = match.AwayTeam.ClubId,
                HomeScore = match.HomeScore,
                AwayScore = match.AwayScore,
                QuarterResults = match.QuarterResults,
                Status = match.Status,
                ResultSubmissionStatus = match.ResultSubmissionStatus,
                IsResultConfirmed = match.IsResultConfirmed,
                ActualScheduledDate = match.ActualScheduledDate,
                Venue = match.VenueOverride ?? match.HomeTeam.Club?.VenueName,
                LeagueStartDate = match.League.StartDate,
                LeagueEndDate = match.League.EndDate,
                LeagueBreaks = leagueBreaks,
                DisputeReason = match.DisputeReason,
                PendingReschedule = pendingReschedule == null ? null : new RescheduleRequestDTO
                {
                    Id = pendingReschedule.Id,
                    ProposedDate = pendingReschedule.ProposedDate,
                    Reason = pendingReschedule.Reason,
                    ProposerClubId = pendingReschedule.RequestedByClubId,
                    ProposerIsHome = pendingReschedule.RequestedByClubId == match.HomeTeam.ClubId,
                    ExpiresAt = pendingReschedule.ExpiresAt
                },
                HomeTeamStats = BuildPlayerStats(homeRoster, existingStats),
                AwayTeamStats = BuildPlayerStats(awayRoster, existingStats)
            };

            return QueryResponse<MatchWithStatsDTO>.Success(dto);
        }

        private static List<PlayerMatchStatDTO> BuildPlayerStats(
            List<PlayerTeamHistory> roster,
            Dictionary<Guid, Domain.PlayerMatchStats> existingStats)
        {
            return roster.Select(pth =>
            {
                existingStats.TryGetValue(pth.PlayerId, out var stat);
                return new PlayerMatchStatDTO
                {
                    PlayerId = pth.PlayerId,
                    PlayerName = $"{pth.Player.FirstName} {pth.Player.LastName}",
                    JerseyNumber = pth.JerseyNumber,
                    Points = stat?.Points ?? 0,
                    ThreePointers = stat?.ThreePointers ?? 0,
                    Fouls = stat?.Fouls ?? 0,
                    DidNotPlay = stat?.DidNotPlay ?? false,
                    StatsEntered = stat != null
                };
            }).ToList();
        }
    }
}
