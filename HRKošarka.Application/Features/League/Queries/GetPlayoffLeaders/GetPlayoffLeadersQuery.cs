using HRKošarka.Application.Features.League.Queries.GetLeagueStandings;
using HRKošarka.Application.Models.Responses;
using MediatR;

namespace HRKošarka.Application.Features.League.Queries.GetPlayoffLeaders
{
    public class GetPlayoffLeadersQuery : IRequest<QueryResponse<LeagueLeadersDTO?>>
    {
        public Guid LeagueId { get; set; }

        public GetPlayoffLeadersQuery(Guid leagueId) => LeagueId = leagueId;
    }
}
