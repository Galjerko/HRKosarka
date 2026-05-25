using HRKošarka.Application.Models.Responses;
using MediatR;

namespace HRKošarka.Application.Features.Match.Commands.ProposeReschedule
{
    public class ProposeRescheduleCommand : IRequest<CommandResponse<bool>>
    {
        public Guid MatchId { get; set; }
        public DateTime ProposedDate { get; set; }
        public string Reason { get; set; } = string.Empty;
        public Guid? ProposerClubId { get; set; }
        public string? ProposerUserId { get; set; }
    }
}
