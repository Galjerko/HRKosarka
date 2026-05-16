using HRKošarka.Application.Models.Responses;
using MediatR;

namespace HRKošarka.Application.Features.League.Commands.GenerateLeagueSchedule
{
    public class GenerateLeagueScheduleCommand : IRequest<CommandResponse<int>>
    {
        public GenerateLeagueScheduleCommand(Guid leagueId) => LeagueId = leagueId;
        public Guid LeagueId { get; }
    }
}
