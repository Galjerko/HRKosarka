using FluentValidation;

namespace HRKošarka.Application.Features.Team.Commands.AssignTeamRepresentative
{
    public class AssignTeamRepresentativeCommandValidator : AbstractValidator<AssignTeamRepresentativeCommand>
    {
        public AssignTeamRepresentativeCommandValidator()
        {
            RuleFor(x => x.TeamId).NotEmpty().WithMessage("Team is required.");
            RuleFor(x => x.UserId).NotEmpty().WithMessage("User is required.");
        }
    }
}
