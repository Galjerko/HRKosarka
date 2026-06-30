using FluentValidation;

namespace HRKošarka.Application.Features.League.Commands.UpdateLeague
{
    public class UpdateLeagueCommandValidator : AbstractValidator<UpdateLeagueCommand>
    {
        public UpdateLeagueCommandValidator()
        {
            RuleFor(x => x.Id)
                .NotEmpty().WithMessage("League Id is required.");

            RuleFor(x => x.Name)
                .NotEmpty().WithMessage("League name is required.")
                .MaximumLength(150).WithMessage("League name must not exceed 150 characters.");

            RuleFor(x => x.SeasonId)
                .NotEmpty().WithMessage("Season is required.");

            RuleFor(x => x.AgeCategoryId)
                .NotEmpty().WithMessage("Age category is required.");

            RuleFor(x => x.StartDate)
                .NotEmpty().WithMessage("Start date is required.");

            RuleFor(x => x.EndDate)
                .NotEmpty().WithMessage("End date is required.")
                .GreaterThanOrEqualTo(x => x.StartDate)
                .WithMessage("End date must be on or after start date.");

            RuleFor(x => x.NumberOfRounds)
                .GreaterThanOrEqualTo(1).WithMessage("Number of rounds must be at least 1.");

            RuleFor(x => x.PlayoffTeamCount)
                .NotNull().WithMessage("Playoff team count is required when playoff is enabled.")
                .Must(c => c is 2 or 4 or 8).WithMessage("Playoff team count must be 2, 4, or 8.")
                .When(x => x.HasPlayoff);

            RuleFor(x => x.PlayoffEndDate)
                .GreaterThanOrEqualTo(x => x.StartDate).WithMessage("Playoff end date must be on or after the league start date.")
                .When(x => x.HasPlayoff && x.PlayoffEndDate.HasValue);
        }
    }
}
