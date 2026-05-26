using HRKošarka.Application.Models.Responses;
using MediatR;

namespace HRKošarka.Application.Features.Team.Queries.GetTeamLeagueStanding
{
    public class GetTeamLeagueStandingQuery : IRequest<QueryResponse<TeamLeagueStandingDTO?>>
    {
        public Guid TeamId { get; set; }
        public Guid LeagueId { get; set; }
    }
}
