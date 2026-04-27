using HRKošarka.Application.Contracts.Logging;
using HRKošarka.Application.Contracts.Persistence;
using HRKošarka.Application.Exceptions;
using MediatR;

namespace HRKošarka.Application.Features.Player.Commands.ActivatePlayer
{
    public class ActivatePlayerCommandHandler : IRequestHandler<ActivatePlayerCommand, Unit>
    {
        private readonly IPlayerRepository _playerRepository;
        private readonly IAppLogger<ActivatePlayerCommandHandler> _logger;

        public ActivatePlayerCommandHandler(IPlayerRepository playerRepository, IAppLogger<ActivatePlayerCommandHandler> logger)
        {
            _playerRepository = playerRepository;
            _logger = logger;
        }

        public async Task<Unit> Handle(ActivatePlayerCommand request, CancellationToken cancellationToken)
        {
            var playerToActivate = await _playerRepository.GetByIdAsync(request.Id, cancellationToken);

            if (playerToActivate == null)
            {
                _logger.LogWarning("Player with ID {Id} not found", request.Id);
                throw new NotFoundException(nameof(Domain.Player), request.Id);
            }

            if (playerToActivate.IsActive)
            {
                _logger.LogInformation("Player with ID {Id} is already active", request.Id);
                throw new BadRequestException("Player is already activated");
            }

            playerToActivate.DeactivateDate = null;
            await _playerRepository.UpdateAsync(playerToActivate, cancellationToken);

            _logger.LogInformation("Player {FirstName} {LastName} activated at {ActivateDate}",
                playerToActivate.FirstName, playerToActivate.LastName, DateTime.Now);

            return Unit.Value;
        }
    }
}
