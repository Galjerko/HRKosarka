using HRKošarka.Application.Models.Responses;
using MediatR;

namespace HRKošarka.Application.Features.Match.Commands.ConfirmMatchResult
{
    public class ConfirmMatchResultCommand : IRequest<CommandResponse<bool>>
    {
        public Guid MatchId { get; set; }
        public string? ConfirmedByUserId { get; set; }
        public bool IsForced { get; set; }
        public string? ConfirmerClubId { get; set; }
        public string? ConfirmerUserId { get; set; }
    }
}
