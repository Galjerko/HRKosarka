using HRKošarka.Application.Models.Responses;
using MediatR;

namespace HRKošarka.Application.Features.Match.Commands.RecordForfeit
{
    public class RecordForfeitCommand : IRequest<CommandResponse<bool>>
    {
        public Guid MatchId { get; set; }
        public Guid ForfeitingTeamId { get; set; }
        public string? ConfirmedByUserId { get; set; }
    }
}
