using FluentValidation;

namespace HRKošarka.Application.Features.Match.Commands.ProposeReschedule
{
    public class ProposeRescheduleCommandValidator : AbstractValidator<ProposeRescheduleCommand>
    {
        public ProposeRescheduleCommandValidator()
        {
            RuleFor(x => x.MatchId).NotEmpty().WithMessage("Match ID is required.");
            RuleFor(x => x.ProposedDate)
                .NotEmpty().WithMessage("Proposed date is required.");
            RuleFor(x => x.Reason)
                .NotEmpty().WithMessage("A reason for rescheduling is required.")
                .MaximumLength(500).WithMessage("Reason cannot exceed 500 characters.");
            RuleFor(x => x).Must(x => x.ProposerClubId.HasValue || !string.IsNullOrEmpty(x.ProposerUserId))
                .WithMessage("Either a club ID or user ID must be provided.");
        }
    }
}
