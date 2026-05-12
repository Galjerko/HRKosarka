using HRKošarka.Application.Models.Responses;
using MediatR;

namespace HRKošarka.Application.Features.Team.Queries.GetTeamLeagues
{
    public class GetTeamLeaguesQuery : IRequest<QueryResponse<List<TeamLeagueDTO>>>
    {
        public GetTeamLeaguesQuery(Guid teamId) => TeamId = teamId;
        public Guid TeamId { get; }
    }
}
