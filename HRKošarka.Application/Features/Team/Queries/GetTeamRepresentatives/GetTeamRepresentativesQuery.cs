using HRKošarka.Application.Models.Responses;
using MediatR;

namespace HRKošarka.Application.Features.Team.Queries.GetTeamRepresentatives
{
    public class GetTeamRepresentativesQuery : IRequest<QueryResponse<List<TeamRepresentativeDTO>>>
    {
        public Guid TeamId { get; set; }
        public GetTeamRepresentativesQuery(Guid teamId) => TeamId = teamId;
    }
}
