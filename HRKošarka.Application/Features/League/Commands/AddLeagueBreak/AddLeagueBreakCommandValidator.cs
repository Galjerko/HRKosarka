using FluentValidation;

namespace HRKošarka.Application.Features.League.Commands.AddLeagueBreak
{
    public class AddLeagueBreakCommandValidator : AbstractValidator<AddLeagueBreakCommand>
    {
        public AddLeagueBreakCommandValidator()
        {
            RuleFor(x => x.LeagueId).NotEmpty().WithMessage("League is required.");
            RuleFor(x => x.Name).NotEmpty().WithMessage("Break name is required.")
                .MaximumLength(100).WithMessage("Break name must not exceed 100 characters.");
            RuleFor(x => x.StartDate).NotEmpty().WithMessage("Start date is required.");
            RuleFor(x => x.EndDate).NotEmpty().WithMessage("End date is required.")
                .GreaterThan(x => x.StartDate).WithMessage("End date must be after start date.");
        }
    }
}
