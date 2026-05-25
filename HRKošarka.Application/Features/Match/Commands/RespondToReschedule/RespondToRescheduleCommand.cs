using HRKošarka.Application.Models.Responses;
using MediatR;

namespace HRKošarka.Application.Features.Match.Commands.RespondToReschedule
{
    public class RespondToRescheduleCommand : IRequest<CommandResponse<bool>>
    {
        public Guid MatchId { get; set; }
        public bool Accept { get; set; }
        public Guid? ResponderClubId { get; set; }
        public string? ResponderUserId { get; set; }
    }
}
