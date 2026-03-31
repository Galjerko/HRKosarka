using HRKošarka.Application.Contracts.Logging;
using HRKošarka.Application.Contracts.Persistence;
using HRKošarka.Application.Exceptions;
using MediatR;

namespace HRKošarka.Application.Features.League.Commands.ActivateLeague
{
    public class ActivateLeagueCommandHandler : IRequestHandler<ActivateLeagueCommand, Unit>
    {
        private readonly IGenericRepository<Domain.League> _leagueRepository;
        private readonly IAppLogger<ActivateLeagueCommandHandler> _logger;

        public ActivateLeagueCommandHandler(
            IGenericRepository<Domain.League> leagueRepository,
            IAppLogger<ActivateLeagueCommandHandler> logger)
        {
            _leagueRepository = leagueRepository;
            _logger = logger;
        }

        public async Task<Unit> Handle(ActivateLeagueCommand request, CancellationToken cancellationToken)
        {
            var league = await _leagueRepository.GetByIdAsync(request.Id, cancellationToken);

            if (league is null)
            {
                throw new NotFoundException(nameof(Domain.League), request.Id);
            }

            league.IsActive = true;
            league.DeactivateDate = null;

            await _leagueRepository.UpdateAsync(league, cancellationToken);

            _logger.LogInformation("League {Id} activated successfully.", league.Id);

            return Unit.Value;
        }
    }
}
