using HRKošarka.Application.Models.Responses;
using MediatR;

namespace HRKošarka.Application.Features.League.Queries.GetAvailableTeamsForLeague
{
    public class GetAvailableTeamsForLeagueQuery : IRequest<QueryResponse<List<AvailableTeamForLeagueDTO>>>
    {
        public Guid LeagueId { get; set; }
        public string? SearchTerm { get; set; }
    }
}
