using HRKošarka.Application.Contracts.Logging;
using HRKošarka.Application.Contracts.Persistence;
using HRKošarka.Application.Exceptions;
using MediatR;

namespace HRKošarka.Application.Features.League.Commands.DeleteLeague
{
    public class DeleteLeagueCommandHandler : IRequestHandler<DeleteLeagueCommand, Unit>
    {
        private readonly IGenericRepository<Domain.League> _leagueRepository;
        private readonly IAppLogger<DeleteLeagueCommandHandler> _logger;

        public DeleteLeagueCommandHandler(
            IGenericRepository<Domain.League> leagueRepository,
            IAppLogger<DeleteLeagueCommandHandler> logger)
        {
            _leagueRepository = leagueRepository;
            _logger = logger;
        }

        public async Task<Unit> Handle(DeleteLeagueCommand request, CancellationToken cancellationToken)
        {
            _logger.LogInformation("Attempting to delete league with ID: {Id}", request.Id);

            var league = await _leagueRepository.GetByIdAsync(request.Id, cancellationToken);

            if (league == null)
            {
                _logger.LogWarning("League with ID {Id} not found", request.Id);
                throw new NotFoundException(nameof(Domain.League), request.Id);
            }

            if (league.DateDeleted != null)
            {
                _logger.LogInformation("League with ID {Id} is already deleted", request.Id);
                throw new BadRequestException("League is already deleted");
            }

            await _leagueRepository.DeleteAsync(league.Id, cancellationToken);

            _logger.LogInformation("Successfully deleted league with ID: {Id}", request.Id);

            return Unit.Value;
        }
    }
}
