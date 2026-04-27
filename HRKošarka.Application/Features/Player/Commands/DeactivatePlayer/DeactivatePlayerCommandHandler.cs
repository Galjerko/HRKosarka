using HRKošarka.Application.Contracts.Logging;
using HRKošarka.Application.Contracts.Persistence;
using HRKošarka.Application.Exceptions;
using MediatR;

namespace HRKošarka.Application.Features.Player.Commands.DeactivatePlayer
{
    public class DeactivatePlayerCommandHandler : IRequestHandler<DeactivatePlayerCommand, Unit>
    {
        private readonly IPlayerRepository _playerRepository;
        private readonly IPlayerTeamHistoryRepository _historyRepository;
        private readonly IAppLogger<DeactivatePlayerCommandHandler> _logger;

        public DeactivatePlayerCommandHandler(
            IPlayerRepository playerRepository,
            IPlayerTeamHistoryRepository historyRepository,
            IAppLogger<DeactivatePlayerCommandHandler> logger)
        {
            _playerRepository = playerRepository;
            _historyRepository = historyRepository;
            _logger = logger;
        }

        public async Task<Unit> Handle(DeactivatePlayerCommand request, CancellationToken cancellationToken)
        {
            var playerToDeactivate = await _playerRepository.GetByIdAsync(request.Id, cancellationToken);

            if (playerToDeactivate == null)
            {
                _logger.LogWarning("Player with ID {Id} not found", request.Id);
                throw new NotFoundException(nameof(Domain.Player), request.Id);
            }

            if (!playerToDeactivate.IsActive)
            {
                _logger.LogInformation("Player with ID {Id} is already deactivated", request.Id);
                throw new BadRequestException("Player is already deactivated");
            }

            await _historyRepository.DeactivateAllForPlayerAsync(request.Id, cancellationToken);

            playerToDeactivate.DeactivateDate = DateTime.Now;
            await _playerRepository.UpdateAsync(playerToDeactivate, cancellationToken);

            _logger.LogInformation("Player {FirstName} {LastName} deactivated at {DeactivateDate}",
                playerToDeactivate.FirstName, playerToDeactivate.LastName, playerToDeactivate.DeactivateDate);

            return Unit.Value;
        }
    }
}
