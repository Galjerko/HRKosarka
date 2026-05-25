using HRKošarka.Application.Contracts.Persistence;
using HRKošarka.Application.Exceptions;
using HRKošarka.Application.Models.Responses;
using HRKošarka.Domain;
using HRKošarka.Domain.Common;
using MediatR;

namespace HRKošarka.Application.Features.Match.Commands.SaveMatchStats
{
    public class SaveMatchStatsCommandHandler : IRequestHandler<SaveMatchStatsCommand, CommandResponse<bool>>
    {
        private readonly IMatchRepository _matchRepository;
        private readonly ITeamRepository _teamRepository;
        private readonly IPlayerMatchStatsRepository _statsRepository;
        private readonly ITeamRepresentativeRepository _repRepository;

        public SaveMatchStatsCommandHandler(
            IMatchRepository matchRepository,
            ITeamRepository teamRepository,
            IPlayerMatchStatsRepository statsRepository,
            ITeamRepresentativeRepository repRepository)
        {
            _matchRepository = matchRepository;
            _teamRepository = teamRepository;
            _statsRepository = statsRepository;
            _repRepository = repRepository;
        }

        public async Task<CommandResponse<bool>> Handle(SaveMatchStatsCommand request, CancellationToken ct)
        {
            var validationResult = await new SaveMatchStatsCommandValidator().ValidateAsync(request, ct);
            if (!validationResult.IsValid)
                throw new BadRequestException("Invalid stats data.", validationResult);

            var match = await _matchRepository.GetByIdAsync(request.MatchId, ct)
                ?? throw new NotFoundException("Match", request.MatchId);

            if (match.IsResultConfirmed)
                throw new BadRequestException("Match result is already confirmed and cannot be changed.");

            if (match.ResultSubmissionStatus == ResultSubmissionStatus.Disputed)
                throw new BadRequestException("Match is disputed. An administrator must resolve it first.");

            if (match.HomeTeamId != request.TeamId && match.AwayTeamId != request.TeamId)
                throw new BadRequestException("This team is not playing in this match.");

            if (request.PlayerStats.Count(p => !p.DidNotPlay) < 5)
                throw new BadRequestException("At least 5 players must have played (not DNP) before stats can be saved.");

            bool isAdmin = string.IsNullOrEmpty(request.SubmitterClubId) && string.IsNullOrEmpty(request.SubmitterUserId);
            if (!isAdmin)
            {
                bool authorized = false;
                if (!string.IsNullOrEmpty(request.SubmitterClubId))
                {
                    var team = await _teamRepository.GetByIdAsync(request.TeamId, ct)
                        ?? throw new NotFoundException("Team", request.TeamId);
                    authorized = team.ClubId.ToString() == request.SubmitterClubId;
                }
                if (!authorized && !string.IsNullOrEmpty(request.SubmitterUserId))
                    authorized = await _repRepository.IsActiveRepForTeamAsync(request.SubmitterUserId, request.TeamId, ct);
                if (!authorized)
                    throw new BadRequestException("You can only submit stats for your own team.");
            }

            bool isHomeTeam = match.HomeTeamId == request.TeamId;

            if (isHomeTeam || isAdmin)
            {
                if (request.HomeScore.HasValue) match.HomeScore = request.HomeScore;
                if (request.AwayScore.HasValue) match.AwayScore = request.AwayScore;
                if (request.QuarterResults != null) match.QuarterResults = request.QuarterResults;
            }

            await _matchRepository.UpdateAsync(match, ct);

            foreach (var entry in request.PlayerStats)
            {
                var existing = await _statsRepository.GetByMatchAndPlayerAsync(request.MatchId, entry.PlayerId, ct);

                if (existing != null)
                {
                    existing.Points = entry.DidNotPlay ? 0 : entry.Points;
                    existing.ThreePointers = entry.DidNotPlay ? 0 : entry.ThreePointers;
                    existing.Fouls = entry.DidNotPlay ? 0 : entry.Fouls;
                    existing.DidNotPlay = entry.DidNotPlay;
                    existing.TeamId = request.TeamId;
                    await _statsRepository.UpdateAsync(existing, ct);
                }
                else
                {
                    await _statsRepository.CreateAsync(new PlayerMatchStats
                    {
                        MatchId = request.MatchId,
                        PlayerId = entry.PlayerId,
                        TeamId = request.TeamId,
                        Points = entry.DidNotPlay ? 0 : entry.Points,
                        ThreePointers = entry.DidNotPlay ? 0 : entry.ThreePointers,
                        Fouls = entry.DidNotPlay ? 0 : entry.Fouls,
                        DidNotPlay = entry.DidNotPlay
                    }, ct);
                }
            }

            return CommandResponse<bool>.Success(true, "Stats saved successfully.");
        }
    }
}
