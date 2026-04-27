using HRKošarka.Application.Models.Responses;
using MediatR;

namespace HRKošarka.Application.Features.Team.Queries.GetAvailableTeamsForPlayer
{
    public class GetAvailableTeamsForPlayerQuery : IRequest<QueryResponse<List<AvailableTeamDTO>>>
    {
        public Guid PlayerId { get; set; }
        public string? SearchTerm { get; set; }
    }
}
