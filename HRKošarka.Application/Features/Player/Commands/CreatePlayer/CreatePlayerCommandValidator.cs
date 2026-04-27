using FluentValidation;
using HRKošarka.Application.Contracts.Persistence;
using HRKošarka.Domain.Common;

namespace HRKošarka.Application.Features.Player.Commands.CreatePlayer
{
    public class CreatePlayerCommandValidator : AbstractValidator<CreatePlayerCommand>
    {
        private readonly IPlayerRepository _playerRepository;

        public CreatePlayerCommandValidator(IPlayerRepository playerRepository)
        {
            _playerRepository = playerRepository;

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

            RuleFor(p => p.Gender)
                .IsInEnum().WithMessage("Invalid gender value.");

            RuleFor(p => p.Nationality)
                .MaximumLength(50).WithMessage("Nationality must not exceed 50 characters.")
                .When(p => !string.IsNullOrEmpty(p.Nationality));

            RuleFor(p => p.ImageName)
                .MaximumLength(255).WithMessage("Image name must not exceed 255 characters.")
                .When(p => !string.IsNullOrEmpty(p.ImageName));

            RuleFor(p => p)
                .MustAsync(RegistrationNumberUnique)
                .WithMessage("Player with that registration number already exists.");
        }

        private async Task<bool> RegistrationNumberUnique(CreatePlayerCommand command, CancellationToken token)
        {
            return await _playerRepository.IsRegistrationNumberUnique(command.RegistrationNumber, null, token);
        }
    }
}
