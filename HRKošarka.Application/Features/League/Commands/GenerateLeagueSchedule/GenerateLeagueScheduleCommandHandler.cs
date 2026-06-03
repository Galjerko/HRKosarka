using HRKošarka.Application.Contracts.Logging;
using HRKošarka.Application.Contracts.Persistence;
using HRKošarka.Application.Exceptions;
using HRKošarka.Application.Models.Responses;
using HRKošarka.Domain;
using HRKošarka.Domain.Common;
using DomainMatch = HRKošarka.Domain.Match;
using HRKošarka.Domain.Helpers;
using MediatR;

namespace HRKošarka.Application.Features.League.Commands.GenerateLeagueSchedule
{
    public class GenerateLeagueScheduleCommandHandler : IRequestHandler<GenerateLeagueScheduleCommand, CommandResponse<int>>
    {
        private readonly ILeagueRepository _leagueRepository;
        private readonly IGenericRepository<DomainMatch> _matchRepository;
        private readonly IAppLogger<GenerateLeagueScheduleCommandHandler> _logger;

        public GenerateLeagueScheduleCommandHandler(
            ILeagueRepository leagueRepository,
            IGenericRepository<DomainMatch> matchRepository,
            IAppLogger<GenerateLeagueScheduleCommandHandler> logger)
        {
            _leagueRepository = leagueRepository;
            _matchRepository = matchRepository;
            _logger = logger;
        }

        public async Task<CommandResponse<int>> Handle(GenerateLeagueScheduleCommand request, CancellationToken cancellationToken)
        {
            _logger.LogInformation("Attempting to generate schedule for league {LeagueId}", request.LeagueId);

            var league = await _leagueRepository.GetLeagueWithDetailsAsync(request.LeagueId, cancellationToken)
                ?? throw new NotFoundException(nameof(Domain.League), request.LeagueId);

            if (!league.IsActive)
                throw new BadRequestException("Cannot generate a schedule for an inactive league.");
            if (league.ScheduleGenerated)
                throw new BadRequestException("Schedule has already been generated for this league.");
            var registeredTeams = await _leagueRepository.GetLeagueTeamsAsync(request.LeagueId, cancellationToken);
            if (registeredTeams.Count < 2)
                throw new BadRequestException("At least 2 teams must be registered before generating a schedule.");

            var breaks = await _leagueRepository.GetLeagueBreaksAsync(request.LeagueId, cancellationToken);
            var breakRanges = breaks.Select(b => (b.StartDate, b.EndDate)).ToList();
            var teamIds = registeredTeams.Select(t => t.TeamId).ToList();

            if (league.CompetitionType == CompetitionType.Cup)
            {
                var cupSlots = CupBracketScheduler.GenerateRound1(teamIds, league.StartDate, breakRanges);
                var cupMatches = cupSlots.Select(s => new DomainMatch
                {
                    LeagueId = request.LeagueId,
                    HomeTeamId = s.HomeTeamId,
                    AwayTeamId = s.AwayTeamId,
                    Round = s.Round,
                    RoundName = s.RoundName,
                    DefaultScheduledDate = s.Date,
                    ActualScheduledDate = s.Date,
                    Status = MatchStatus.Scheduled,
                    SchedulingStatus = SchedulingStatus.Default,
                    LastSchedulingUpdate = DateTime.Now
                }).ToList();

                await _matchRepository.CreateRangeAsync(cupMatches, cancellationToken);
                league.ScheduleGenerated = true;
                await _leagueRepository.UpdateAsync(league, cancellationToken);

                _logger.LogInformation("Generated cup draw: {Count} round 1 matches for league {LeagueId}",
                    cupMatches.Count, request.LeagueId);
                return CommandResponse<int>.Success(cupMatches.Count,
                    $"Cup draw generated: {cupMatches.Count} match(es) in round 1.");
            }

            var slots = RoundRobinScheduler.Generate(teamIds, league.StartDate, league.NumberOfRounds, breakRanges);

            var matches = slots.Select(s => new DomainMatch
            {
                LeagueId = request.LeagueId,
                HomeTeamId = s.HomeTeamId,
                AwayTeamId = s.AwayTeamId,
                Round = s.Round,
                RoundName = s.RoundName,
                DefaultScheduledDate = s.Date,
                ActualScheduledDate = s.Date,
                Status = MatchStatus.Scheduled,
                SchedulingStatus = SchedulingStatus.Default,
                LastSchedulingUpdate = DateTime.Now
            }).ToList();

            await _matchRepository.CreateRangeAsync(matches, cancellationToken);

            league.ScheduleGenerated = true;
            await _leagueRepository.UpdateAsync(league, cancellationToken);

            var roundCount = slots.Select(s => s.Round).Distinct().Count();
            _logger.LogInformation(
                "Generated {Count} matches across {Rounds} rounds for league {LeagueId}",
                matches.Count, roundCount, request.LeagueId);

            return CommandResponse<int>.Success(matches.Count,
                $"Schedule generated: {matches.Count} matches across {roundCount} rounds.");
        }
    }
}
