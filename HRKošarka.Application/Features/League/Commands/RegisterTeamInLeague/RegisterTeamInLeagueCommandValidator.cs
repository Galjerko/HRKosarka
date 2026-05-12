using FluentValidation;

namespace HRKošarka.Application.Features.League.Commands.RegisterTeamInLeague
{
    public class RegisterTeamInLeagueCommandValidator : AbstractValidator<RegisterTeamInLeagueCommand>
    {
        public RegisterTeamInLeagueCommandValidator()
        {
            RuleFor(x => x.LeagueId).NotEmpty().WithMessage("League is required.");
            RuleFor(x => x.TeamId).NotEmpty().WithMessage("Team is required.");
            RuleFor(x => x.RegistrationDate).NotEmpty().WithMessage("Registration date is required.");
        }
    }
}
