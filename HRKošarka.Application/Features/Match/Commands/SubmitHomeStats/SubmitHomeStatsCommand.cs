using HRKošarka.Application.Models.Responses;
using MediatR;

namespace HRKošarka.Application.Features.Match.Commands.SubmitHomeStats
{
    public class SubmitHomeStatsCommand : IRequest<CommandResponse<bool>>
    {
        public Guid MatchId { get; set; }
        public string? SubmitterClubId { get; set; }
    }
}
