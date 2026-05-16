using HRKošarka.Application.Models.Responses;
using MediatR;

namespace HRKošarka.Application.Features.League.Queries.GetLeagueBreaks
{
    public class GetLeagueBreaksQuery : IRequest<QueryResponse<List<LeagueBreakDTO>>>
    {
        public GetLeagueBreaksQuery(Guid leagueId) => LeagueId = leagueId;
        public Guid LeagueId { get; }
    }
}
