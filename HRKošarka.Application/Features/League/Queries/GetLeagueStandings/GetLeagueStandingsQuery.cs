using HRKošarka.Application.Models.Responses;
using MediatR;

namespace HRKošarka.Application.Features.League.Queries.GetLeagueStandings
{
    public class GetLeagueStandingsQuery : IRequest<QueryResponse<LeagueStandingsDTO>>
    {
        public Guid LeagueId { get; set; }
        public GetLeagueStandingsQuery(Guid leagueId) => LeagueId = leagueId;
    }
}
