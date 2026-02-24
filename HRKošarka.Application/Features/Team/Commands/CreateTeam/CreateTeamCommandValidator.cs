using FluentValidation;
using HRKošarka.Application.Contracts.Persistence;

namespace HRKošarka.Application.Features.Team.Commands.CreateTeam
{
    public class CreateTeamCommandValidator : AbstractValidator<CreateTeamCommand>
    {
        private readonly ITeamRepository _teamRepository;

        public CreateTeamCommandValidator(ITeamRepository teamRepository)
        {
            _teamRepository = teamRepository;

            RuleFor(t => t.Name)
                .NotEmpty().WithMessage("Name is required.")
                .MaximumLength(150).WithMessage("Name must not exceed 150 characters.");

            RuleFor(t => t.ClubId)
                .NotEmpty().WithMessage("Club ID is required.");

            RuleFor(t => t.AgeCategoryId)
                .NotEmpty().WithMessage("Age Category ID is required.");

            RuleFor(t => t.Gender)
                .IsInEnum().WithMessage("Invalid gender value.");

            RuleFor(t => t)
                .MustAsync(TeamNameUniqueInClubAndAgeCategory)
                .WithMessage("Team with that name already exists in this club for this age category");
        }

        private async Task<bool> TeamNameUniqueInClubAndAgeCategory(CreateTeamCommand command, CancellationToken token)
        {
            return await _teamRepository.IsTeamNameUniqueInClub(command.Name, command.ClubId, command.AgeCategoryId, null, token);
        }
    }
}
