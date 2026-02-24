using FluentValidation;

namespace HRKošarka.Application.Features.Season.Commands.CreateSeason
{
    public class CreateSeasonCommandValidator : AbstractValidator<CreateSeasonCommand>
    {
        public CreateSeasonCommandValidator()
        {
            RuleFor(x => x.Name)
                .NotEmpty().WithMessage("Season name is required.")
                .MaximumLength(10).WithMessage("Season name must not exceed 10 characters.");

            RuleFor(x => x.StartDate)
                .NotEmpty().WithMessage("Start date is required.");

            RuleFor(x => x.EndDate)
                .NotEmpty().WithMessage("End date is required.")
                .GreaterThan(x => x.StartDate)
                .WithMessage("End date must be after start date.");
        }
    }
}