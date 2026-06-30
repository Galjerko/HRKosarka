using HRKošarka.Application.Contracts.Persistence;
using HRKošarka.Application.Exceptions;
using HRKošarka.Application.Models.Responses;
using HRKošarka.Application.Services;
using HRKošarka.Domain.Helpers;
using MediatR;
using System.Text.Json;

namespace HRKošarka.Application.Features.League.Commands.GeneratePlayoff
{
    public class GeneratePlayoffCommandHandler : IRequestHandler<GeneratePlayoffCommand, CommandResponse<bool>>
    {
        private readonly ILeagueRepository _leagueRepository;
        private readonly ILeagueStandingRepository _standingRepository;
        private readonly IPlayoffRepository _playoffRepository;

        public GeneratePlayoffCommandHandler(
            ILeagueRepository leagueRepository,
            ILeagueStandingRepository standingRepository,
            IPlayoffRepository playoffRepository)
        {
            _leagueRepository = leagueRepository;
            _standingRepository = standingRepository;
            _playoffRepository = playoffRepository;
        }

        public async Task<CommandResponse<bool>> Handle(GeneratePlayoffCommand request, CancellationToken ct)
        {
            var validationResult = await new GeneratePlayoffCommandValidator().ValidateAsync(request, ct);
            if (!validationResult.IsValid)
                throw new BadRequestException("Invalid playoff generation data", validationResult);

            var league = await _leagueRepository.GetLeagueWithDetailsAsync(request.LeagueId, ct)
                ?? throw new NotFoundException(nameof(Domain.League), request.LeagueId);

            if (!league.HasPlayoff)
                throw new BadRequestException("This league is not configured for a playoff.");

            if (!league.ScheduleGenerated)
                throw new BadRequestException("Regular-season schedule must be generated before creating a playoff.");

            if (league.PlayoffGenerated)
                throw new BadRequestException("Playoff has already been generated for this league.");

            if (!league.PlayoffTeamCount.HasValue || !PlayoffBracketShape.IsValidTeamCount(league.PlayoffTeamCount.Value))
                throw new BadRequestException("League must have a valid PlayoffTeamCount (2, 4, or 8).");

            var teamCount = league.PlayoffTeamCount.Value;
            int expectedRounds = PlayoffBracketShape.GetRoundCount(teamCount);

            if (request.RoundWinsNeeded.Count != expectedRounds)
                throw new BadRequestException($"WinsNeeded must be specified for all {expectedRounds} round(s).");

            if (await _leagueRepository.HasUnfinishedRegularSeasonMatchesAsync(league.Id, ct))
                throw new BadRequestException("All regular-season matches must be completed before generating the playoff.");

            var standings = await _standingRepository.GetByLeagueAsync(league.Id, ct);
            var qualified = standings.Where(s => s.MatchesPlayed > 0).Take(teamCount).ToList();

            if (qualified.Count < teamCount)
                throw new BadRequestException(
                    $"Need at least {teamCount} teams with games played, but only {qualified.Count} qualify.");

            // Normalize admin-supplied start date to 19:00; breaks are NOT applied to playoff scheduling
            var firstPlayoffDate = request.PlayoffStartDate.Date.AddHours(19);
            var capDate = league.PlayoffCapDate;

            if (firstPlayoffDate < league.StartDate.Date.AddHours(19))
                throw new BadRequestException(
                    "The playoff start date cannot be before the league start date.");

            if (firstPlayoffDate > capDate)
                throw new BadRequestException(
                    "The playoff start date falls after the league end date. " +
                    "Adjust the playoff end date or choose an earlier start date.");

            var seeds = qualified
                .Select((s, idx) => new PlayoffSeedEntry(s.TeamId, idx + 1))
                .ToList();

            var allSeries = PlayoffBracketShape.BuildFullBracket(
                seeds, teamCount, request.RoundWinsNeeded, request.Include3rdPlace,
                league.Id, firstPlayoffDate, league.DefaultVenue);

            foreach (var match in allSeries.SelectMany(s => s.Matches))
                PlayoffSchedulingGuard.EnsureWithinCapDate(match.DefaultScheduledDate, capDate);

            var winsDict = new Dictionary<string, int>();
            for (int i = 0; i < request.RoundWinsNeeded.Count; i++)
                winsDict[(i + 1).ToString()] = request.RoundWinsNeeded[i];
            if (request.Include3rdPlace && request.RoundWinsNeeded.Count > 0)
                winsDict["3rdPlace"] = request.RoundWinsNeeded.Last();

            league.PlayoffRoundWinsNeeded = JsonSerializer.Serialize(winsDict);
            league.PlayoffGenerated = true;

            await _playoffRepository.CreateInitialBracketAsync(allSeries, league, ct);

            return CommandResponse<bool>.Success(true, $"Playoff bracket generated with {allSeries.Count} series.");
        }
    }
}
