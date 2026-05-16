using HRKošarka.Application.Contracts.Logging;
using HRKošarka.Application.Contracts.Persistence;
using HRKošarka.Application.Exceptions;
using HRKošarka.Domain;
using MediatR;

namespace HRKošarka.Application.Features.League.Commands.RemoveLeagueBreak
{
    public class RemoveLeagueBreakCommandHandler : IRequestHandler<RemoveLeagueBreakCommand, Unit>
    {
        private readonly ILeagueRepository _leagueRepository;
        private readonly IGenericRepository<LeagueBreak> _breakRepository;
        private readonly IAppLogger<RemoveLeagueBreakCommandHandler> _logger;

        public RemoveLeagueBreakCommandHandler(
            ILeagueRepository leagueRepository,
            IGenericRepository<LeagueBreak> breakRepository,
            IAppLogger<RemoveLeagueBreakCommandHandler> logger)
        {
            _leagueRepository = leagueRepository;
            _breakRepository = breakRepository;
            _logger = logger;
        }

        public async Task<Unit> Handle(RemoveLeagueBreakCommand request, CancellationToken cancellationToken)
        {
            _logger.LogInformation("Attempting to remove break with ID: {BreakId}", request.BreakId);

            var leagueBreak = await _breakRepository.GetByIdAsync(request.BreakId, cancellationToken)
                ?? throw new NotFoundException(nameof(LeagueBreak), request.BreakId);

            var league = await _leagueRepository.GetByIdAsync(leagueBreak.LeagueId, cancellationToken)
                ?? throw new NotFoundException(nameof(Domain.League), leagueBreak.LeagueId);

            if (league.ScheduleGenerated)
                throw new BadRequestException("Cannot remove breaks after the schedule has been generated.");

            await _breakRepository.DeleteAsync(leagueBreak.Id, cancellationToken);

            _logger.LogInformation("Break {BreakId} removed from league {LeagueId}", request.BreakId, leagueBreak.LeagueId);

            return Unit.Value;
        }
    }
}
