using FluentValidation;

namespace HRKošarka.Application.Features.League.Commands.GeneratePlayoff
{
    public class GeneratePlayoffCommandValidator : AbstractValidator<GeneratePlayoffCommand>
    {
        public GeneratePlayoffCommandValidator()
        {
            RuleFor(x => x.LeagueId).NotEmpty().WithMessage("League is required.");

            RuleFor(x => x.PlayoffStartDate)
                .NotEmpty().WithMessage("Playoff start date is required.");

            RuleFor(x => x.RoundWinsNeeded)
                .NotEmpty().WithMessage("WinsNeeded must be specified for each round.")
                .Must(list => list.All(w => w >= 2 && w <= 4))
                .WithMessage("Each WinsNeeded value must be between 2 and 4.");
        }
    }
}
