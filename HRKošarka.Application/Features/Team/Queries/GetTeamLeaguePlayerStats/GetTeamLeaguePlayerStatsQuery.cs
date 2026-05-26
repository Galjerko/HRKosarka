using HRKošarka.Application.Models.Responses;
using MediatR;

namespace HRKošarka.Application.Features.Team.Queries.GetTeamLeaguePlayerStats
{
    public class GetTeamLeaguePlayerStatsQuery : IRequest<QueryResponse<List<TeamPlayerStatDTO>>>
    {
        public Guid TeamId { get; set; }
        public Guid LeagueId { get; set; }
    }
}
