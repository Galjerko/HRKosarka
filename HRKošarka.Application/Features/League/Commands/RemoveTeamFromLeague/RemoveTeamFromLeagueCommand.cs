using HRKošarka.Application.Models.Responses;
using MediatR;

namespace HRKošarka.Application.Features.League.Commands.RemoveTeamFromLeague
{
    public class RemoveTeamFromLeagueCommand : IRequest<CommandResponse<bool>>
    {
        public Guid LeagueId { get; set; }
        public Guid TeamId { get; set; }
    }
}
