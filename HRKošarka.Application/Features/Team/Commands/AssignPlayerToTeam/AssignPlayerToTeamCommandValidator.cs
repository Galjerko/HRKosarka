using FluentValidation;

namespace HRKošarka.Application.Features.Team.Commands.AssignPlayerToTeam
{
    public class AssignPlayerToTeamCommandValidator : AbstractValidator<AssignPlayerToTeamCommand>
    {
        public AssignPlayerToTeamCommandValidator()
        {
            RuleFor(x => x.PlayerId)
                .NotEmpty().WithMessage("Player is required.");

            RuleFor(x => x.SeasonId)
                .NotEmpty().WithMessage("Season is required.");

            RuleFor(x => x.JoinDate)
                .NotEmpty().WithMessage("Join date is required.");

            RuleFor(x => x.JerseyNumber)
                .InclusiveBetween(0, 99).WithMessage("Jersey number must be between 0 and 99.")
                .When(x => x.JerseyNumber.HasValue);
        }
    }
}
