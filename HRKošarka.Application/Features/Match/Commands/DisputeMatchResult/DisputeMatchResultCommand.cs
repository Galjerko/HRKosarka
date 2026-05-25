using HRKošarka.Application.Models.Responses;
using MediatR;

namespace HRKošarka.Application.Features.Match.Commands.DisputeMatchResult
{
    public class DisputeMatchResultCommand : IRequest<CommandResponse<bool>>
    {
        public Guid MatchId { get; set; }
        public string? DisputerClubId { get; set; }
        public string? DisputerUserId { get; set; }
        public string Reason { get; set; } = string.Empty;
    }
}
