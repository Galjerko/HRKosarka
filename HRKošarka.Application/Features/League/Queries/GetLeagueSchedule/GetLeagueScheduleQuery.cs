using HRKošarka.Application.Models.Responses;
using MediatR;

namespace HRKošarka.Application.Features.League.Queries.GetLeagueSchedule
{
    public class GetLeagueScheduleQuery : IRequest<QueryResponse<List<LeagueRoundDTO>>>
    {
        public GetLeagueScheduleQuery(Guid leagueId) => LeagueId = leagueId;
        public Guid LeagueId { get; }
    }
}
