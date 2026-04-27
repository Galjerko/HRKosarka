using HRKošarka.Application.Contracts.Logging;
using HRKošarka.Application.Contracts.Persistence;
using HRKošarka.Application.Exceptions;
using MediatR;

namespace HRKošarka.Application.Features.Player.Commands.UpdatePlayer
{
    public class UpdatePlayerCommandHandler : IRequestHandler<UpdatePlayerCommand, Unit>
    {
        private readonly IPlayerRepository _playerRepository;
        private readonly IAppLogger<UpdatePlayerCommandHandler> _logger;

        public UpdatePlayerCommandHandler(IPlayerRepository playerRepository, IAppLogger<UpdatePlayerCommandHandler> logger)
        {
            _playerRepository = playerRepository;
            _logger = logger;
        }

        public async Task<Unit> Handle(UpdatePlayerCommand request, CancellationToken cancellationToken)
        {
            var validator = new UpdatePlayerCommandValidator(_playerRepository);
            var validationResult = await validator.ValidateAsync(request, cancellationToken);

            if (!validationResult.IsValid)
            {
                _logger.LogWarning("Validation errors in update request for {0} - {1}", nameof(Domain.Player), request.Id);
                throw new BadRequestException("Invalid Player", validationResult);
            }

            var playerToUpdate = await _playerRepository.GetByIdAsync(request.Id, cancellationToken);
            if (playerToUpdate == null)
                throw new NotFoundException(nameof(Domain.Player), request.Id);

            playerToUpdate.FirstName = request.FirstName;
            playerToUpdate.LastName = request.LastName;
            playerToUpdate.RegistrationNumber = request.RegistrationNumber;
            playerToUpdate.DateOfBirth = request.DateOfBirth;
            playerToUpdate.Height = request.Height;
            playerToUpdate.Weight = request.Weight;
            playerToUpdate.Position = request.Position;
            playerToUpdate.Nationality = request.Nationality;
            playerToUpdate.ImageName = request.ImageName;
            playerToUpdate.ImageContentType = request.ImageContentType;
            playerToUpdate.ImageBytes = request.ImageBytes;
            playerToUpdate.DateModified = DateTime.Now;

            await _playerRepository.UpdateAsync(playerToUpdate, cancellationToken);

            _logger.LogInformation("Player {FirstName} {LastName} (ID: {PlayerId}) successfully updated",
                playerToUpdate.FirstName, playerToUpdate.LastName, playerToUpdate.Id);

            return Unit.Value;
        }
    }
}
