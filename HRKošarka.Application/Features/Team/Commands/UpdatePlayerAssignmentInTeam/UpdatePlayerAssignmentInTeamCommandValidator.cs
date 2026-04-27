using FluentValidation;

namespace HRKošarka.Application.Features.Team.Commands.UpdatePlayerAssignmentInTeam
{
    public class UpdatePlayerAssignmentInTeamCommandValidator : AbstractValidator<UpdatePlayerAssignmentInTeamCommand>
    {
        public UpdatePlayerAssignmentInTeamCommandValidator()
        {
            RuleFor(x => x.TeamId)
                .NotEmpty().WithMessage("Team is required.");

            RuleFor(x => x.PlayerId)
                .NotEmpty().WithMessage("Player is required.");

            RuleFor(x => x.JerseyNumber)
                .InclusiveBetween(0, 99).WithMessage("Jersey number must be between 0 and 99.")
                .When(x => x.JerseyNumber.HasValue);
        }
    }
}
