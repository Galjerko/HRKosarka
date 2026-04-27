using HRKošarka.Application.Contracts.Logging;
using HRKošarka.Application.Contracts.Persistence;
using HRKošarka.Application.Exceptions;
using MediatR;

namespace HRKošarka.Application.Features.Player.Commands.DeletePlayer
{
    public class DeletePlayerCommandHandler : IRequestHandler<DeletePlayerCommand, Unit>
    {
        private readonly IPlayerRepository _playerRepository;
        private readonly IPlayerTeamHistoryRepository _historyRepository;
        private readonly IAppLogger<DeletePlayerCommandHandler> _logger;

        public DeletePlayerCommandHandler(
            IPlayerRepository playerRepository,
            IPlayerTeamHistoryRepository historyRepository,
            IAppLogger<DeletePlayerCommandHandler> logger)
        {
            _playerRepository = playerRepository;
            _historyRepository = historyRepository;
            _logger = logger;
        }

        public async Task<Unit> Handle(DeletePlayerCommand request, CancellationToken cancellationToken)
        {
            _logger.LogInformation("Attempting to delete player with ID: {Id}", request.Id);

            var playerToDelete = await _playerRepository.GetByIdAsync(request.Id, cancellationToken);

            if (playerToDelete == null)
            {
                _logger.LogWarning("Player with ID {Id} not found", request.Id);
                throw new NotFoundException(nameof(Domain.Player), request.Id);
            }

            if (playerToDelete.DateDeleted != null)
            {
                _logger.LogInformation("Player with ID {Id} is already deleted", request.Id);
                throw new BadRequestException("Player is already deleted");
            }

            if (await _historyRepository.HasActiveAssignmentsForPlayerAsync(request.Id, cancellationToken))
                throw new BadRequestException("Cannot delete a player who has active team assignments. Remove them from all teams first.");

            await _playerRepository.DeleteAsync(playerToDelete.Id, cancellationToken);

            _logger.LogInformation("Successfully deleted player with ID: {Id}", request.Id);

            return Unit.Value;
        }
    }
}
