using FluentValidation;
using HRKošarka.Application.Contracts.Persistence;

namespace HRKošarka.Application.Features.Club.Commands.CreateClub
{
    public class CreateClubCommandValidator : AbstractValidator<CreateClubCommand>
    {
        private readonly IClubRepository _clubRepository;

        public CreateClubCommandValidator(IClubRepository clubRepository)
        {
            _clubRepository = clubRepository;

            RuleFor(c => c.Name)
                .NotEmpty().WithMessage("Name is required.")
                .MaximumLength(150).WithMessage("Name must not exceed 150 characters.");

            RuleFor(c => c.Description)
                .MaximumLength(1000).WithMessage("Description must not exceed 1000 characters.")
                .When(c => !string.IsNullOrEmpty(c.Description));

            RuleFor(c => c.Address)
                .MaximumLength(200).WithMessage("Address must not exceed 200 characters.")
                .When(c => !string.IsNullOrEmpty(c.Address));

            RuleFor(c => c.City)
                .MaximumLength(100).WithMessage("City must not exceed 100 characters.")
                .When(c => !string.IsNullOrEmpty(c.City));

            RuleFor(c => c.PhoneNumber)
                .MaximumLength(20).WithMessage("Phone number must not exceed 20 characters.")
                .Matches(@"^\+?[1-9]\d{1,14}$").WithMessage("Invalid phone number format.")
                .When(c => !string.IsNullOrEmpty(c.PhoneNumber));

            RuleFor(c => c.Email)
                .MaximumLength(255).WithMessage("Email must not exceed 255 characters.")
                .EmailAddress().WithMessage("Invalid email format.")
                .When(c => !string.IsNullOrEmpty(c.Email));

            RuleFor(c => c.Website)
                .MaximumLength(255).WithMessage("Website must not exceed 255 characters.")
                .When(c => !string.IsNullOrEmpty(c.Website));

            RuleFor(c => c.PostalCode)
                .MaximumLength(20).WithMessage("Postal code must not exceed 20 characters.")
                .When(c => !string.IsNullOrEmpty(c.PostalCode));


            RuleFor(c => c.VenueName)
                .MaximumLength(200).WithMessage("Venue name must not exceed 200 characters.")
                .When(c => !string.IsNullOrEmpty(c.VenueName));

            RuleFor(c => c.FoundedYear)
                .NotEmpty().WithMessage("Founded year is required.")
                .LessThanOrEqualTo(DateTime.Now).WithMessage("Founded year cannot be in the future.");

            RuleFor(c => c)
                .MustAsync(ClubNameUnique)
                .WithMessage("Club with that name already exists");

            RuleFor(c => c.ImageName)
                 .MaximumLength(255).WithMessage("Image name must not exceed 255 characters.")
                 .When(c => !string.IsNullOrEmpty(c.ImageName));
        }

        private async Task<bool> ClubNameUnique(CreateClubCommand command, CancellationToken token)
        {
            return await _clubRepository.IsClubNameUnique(command.Name, null, token);
        }

    }
}
