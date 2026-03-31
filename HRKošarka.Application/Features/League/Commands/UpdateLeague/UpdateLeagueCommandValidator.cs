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
                .GreaterThan(x => x.StartDate)
                .WithMessage("End date must be after start date.");

            RuleFor(x => x.NumberOfRounds)
                .GreaterThanOrEqualTo(1).WithMessage("Number of rounds must be at least 1.");
        }
    }
}
