using HRKošarka.Application.Contracts.Logging;
using HRKošarka.Application.Contracts.Persistence;
using HRKošarka.Application.Exceptions;
using MediatR;

namespace HRKošarka.Application.Features.League.Commands.DeactivateLeague
{
    public class DeactivateLeagueCommandHandler : IRequestHandler<DeactivateLeagueCommand, Unit>
    {
        private readonly IGenericRepository<Domain.League> _leagueRepository;
        private readonly IAppLogger<DeactivateLeagueCommandHandler> _logger;

        public DeactivateLeagueCommandHandler(
            IGenericRepository<Domain.League> leagueRepository,
            IAppLogger<DeactivateLeagueCommandHandler> logger)
        {
            _leagueRepository = leagueRepository;
            _logger = logger;
        }

        public async Task<Unit> Handle(DeactivateLeagueCommand request, CancellationToken cancellationToken)
        {
            var league = await _leagueRepository.GetByIdAsync(request.Id, cancellationToken);

            if (league is null)
            {
                throw new NotFoundException(nameof(Domain.League), request.Id);
            }

            league.IsActive = false;
            league.DeactivateDate = DateTime.Now;

            await _leagueRepository.UpdateAsync(league, cancellationToken);

            _logger.LogInformation("League {Id} deactivated successfully.", league.Id);

            return Unit.Value;
        }
    }
}
