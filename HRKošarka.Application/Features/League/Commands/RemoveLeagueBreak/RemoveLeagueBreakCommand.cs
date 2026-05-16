using MediatR;

namespace HRKošarka.Application.Features.League.Commands.RemoveLeagueBreak
{
    public class RemoveLeagueBreakCommand : IRequest<Unit>
    {
        public RemoveLeagueBreakCommand(Guid breakId) => BreakId = breakId;
        public Guid BreakId { get; }
    }
}
