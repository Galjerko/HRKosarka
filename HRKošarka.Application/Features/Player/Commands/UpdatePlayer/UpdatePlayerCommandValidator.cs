using FluentValidation;
using HRKošarka.Application.Contracts.Persistence;

namespace HRKošarka.Application.Features.Player.Commands.UpdatePlayer
{
    public class UpdatePlayerCommandValidator : AbstractValidator<UpdatePlayerCommand>
    {
        private readonly IPlayerRepository _playerRepository;

        public UpdatePlayerCommandValidator(IPlayerRepository playerRepository)
        {
            _playerRepository = playerRepository;

            RuleFor(p => p.Id)
                .NotEmpty().WithMessage("Player ID is required.")
                .MustAsync(PlayerMustExist).WithMessage("Player not found.")
                .MustAsync(PlayerIsNotDeactivatedOrDeleted).WithMessage("Player is already deactivated or deleted.");

            RuleFor(p => p.FirstName)
                .NotEmpty().WithMessage("First name is required.")
                .MaximumLength(100).WithMessage("First name must not exceed 100 characters.");

            RuleFor(p => p.LastName)
                .NotEmpty().WithMessage("Last name is required.")
                .MaximumLength(100).WithMessage("Last name must not exceed 100 characters.");

            RuleFor(p => p.RegistrationNumber)
                .NotEmpty().WithMessage("Registration number is required.");

            RuleFor(p => p.DateOfBirth)
                .NotEmpty().WithMessage("Date of birth is required.")
                .LessThan(DateTime.Today).WithMessage("Date of birth must be in the past.");

            RuleFor(p => p.Height)
                .InclusiveBetween(100, 250).WithMessage("Height must be between 100 and 250 cm.")
                .When(p => p.Height.HasValue);

            RuleFor(p => p.Weight)
                .InclusiveBetween(30, 200).WithMessage("Weight must be between 30 and 200 kg.")
                .When(p => p.Weight.HasValue);

            RuleFor(p => p.Nationality)
                .MaximumLength(50).WithMessage("Nationality must not exceed 50 characters.")
                .When(p => !string.IsNullOrEmpty(p.Nationality));

            RuleFor(p => p.ImageName)
                .MaximumLength(255).WithMessage("Image name must not exceed 255 characters.")
                .When(p => !string.IsNullOrEmpty(p.ImageName));

            RuleFor(p => p)
                .MustAsync(RegistrationNumberUniqueForUpdate)
                .WithMessage("Player with that registration number already exists.");
        }

        private async Task<bool> PlayerMustExist(Guid id, CancellationToken token)
        {
            return await _playerRepository.GetByIdAsync(id, token) != null;
        }

        private async Task<bool> PlayerIsNotDeactivatedOrDeleted(Guid id, CancellationToken token)
        {
            var player = await _playerRepository.GetByIdAsync(id, token);
            return player != null && player.IsActive && !player.DateDeleted.HasValue;
        }

        private async Task<bool> RegistrationNumberUniqueForUpdate(UpdatePlayerCommand command, CancellationToken token)
        {
            return await _playerRepository.IsRegistrationNumberUnique(command.RegistrationNumber, command.Id, token);
        }
    }
}
