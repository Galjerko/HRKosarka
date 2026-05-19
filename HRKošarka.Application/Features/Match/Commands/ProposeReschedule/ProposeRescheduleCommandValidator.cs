using FluentValidation;

namespace HRKošarka.Application.Features.Match.Commands.ProposeReschedule
{
    public class ProposeRescheduleCommandValidator : AbstractValidator<ProposeRescheduleCommand>
    {
        public ProposeRescheduleCommandValidator()
        {
            RuleFor(x => x.MatchId).NotEmpty().WithMessage("Match ID is required.");
            RuleFor(x => x.ProposedDate)
                .NotEmpty().WithMessage("Proposed date is required.")
                .GreaterThan(DateTime.UtcNow).WithMessage("Proposed date must be in the future.");
            RuleFor(x => x.Reason)
                .NotEmpty().WithMessage("A reason for rescheduling is required.")
                .MaximumLength(500).WithMessage("Reason cannot exceed 500 characters.");
            RuleFor(x => x.ProposerClubId).NotEmpty().WithMessage("Proposer club is required.");
            RuleFor(x => x.ProposerUserId).NotEmpty().WithMessage("Proposer user is required.");
        }
    }
}
