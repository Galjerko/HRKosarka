using HRKošarka.Application.Models.Responses;
using MediatR;

namespace HRKošarka.Application.Features.League.Queries.GetLeagueTeams
{
    public class GetLeagueTeamsQuery : IRequest<QueryResponse<List<LeagueTeamDTO>>>
    {
        public Guid LeagueId { get; set; }
        public GetLeagueTeamsQuery(Guid leagueId) => LeagueId = leagueId;
    }
}
