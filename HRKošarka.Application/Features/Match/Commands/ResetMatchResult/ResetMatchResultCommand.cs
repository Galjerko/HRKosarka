using HRKošarka.Application.Models.Responses;
using MediatR;

namespace HRKošarka.Application.Features.Match.Commands.ResetMatchResult
{
    public class ResetMatchResultCommand : IRequest<CommandResponse<bool>>
    {
        public Guid MatchId { get; set; }
    }
}
