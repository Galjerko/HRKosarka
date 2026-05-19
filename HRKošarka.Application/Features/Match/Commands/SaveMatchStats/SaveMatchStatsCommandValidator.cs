using FluentValidation;

namespace HRKošarka.Application.Features.Match.Commands.SaveMatchStats
{
    public class SaveMatchStatsCommandValidator : AbstractValidator<SaveMatchStatsCommand>
    {
        public SaveMatchStatsCommandValidator()
        {
            RuleFor(x => x.MatchId).NotEmpty().WithMessage("Match ID is required.");
            RuleFor(x => x.TeamId).NotEmpty().WithMessage("Team ID is required.");
            RuleFor(x => x.HomeScore).GreaterThanOrEqualTo(0).When(x => x.HomeScore.HasValue)
                .WithMessage("Home score cannot be negative.");
            RuleFor(x => x.AwayScore).GreaterThanOrEqualTo(0).When(x => x.AwayScore.HasValue)
                .WithMessage("Away score cannot be negative.");
            RuleForEach(x => x.PlayerStats).ChildRules(stat =>
            {
                stat.RuleFor(s => s.PlayerId).NotEmpty().WithMessage("Player ID is required.");
                stat.RuleFor(s => s.Points).GreaterThanOrEqualTo(0).When(s => !s.DidNotPlay)
                    .WithMessage("Points cannot be negative.");
                stat.RuleFor(s => s.Fouls).GreaterThanOrEqualTo(0).When(s => !s.DidNotPlay)
                    .WithMessage("Fouls cannot be negative.");
                stat.RuleFor(s => s.ThreePointers).GreaterThanOrEqualTo(0).When(s => !s.DidNotPlay)
                    .WithMessage("Three-pointers cannot be negative.");
            });
        }
    }
}
