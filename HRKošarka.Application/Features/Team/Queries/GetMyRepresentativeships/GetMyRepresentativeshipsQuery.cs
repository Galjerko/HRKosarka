using HRKošarka.Application.Models.Responses;
using MediatR;

namespace HRKošarka.Application.Features.Team.Queries.GetMyRepresentativeships
{
    public class GetMyRepresentativeshipsQuery : IRequest<QueryResponse<List<TeamRepMembershipDTO>>>
    {
        public string UserId { get; set; } = string.Empty;
        public GetMyRepresentativeshipsQuery(string userId) => UserId = userId;
    }
}
