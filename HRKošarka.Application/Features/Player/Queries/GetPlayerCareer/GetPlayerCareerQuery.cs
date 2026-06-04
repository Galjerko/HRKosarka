using HRKošarka.Application.Models.Responses;
using MediatR;

namespace HRKošarka.Application.Features.Player.Queries.GetPlayerCareer
{
    public class GetPlayerCareerQuery : IRequest<QueryResponse<List<PlayerCareerItemDTO>>>
    {
        public Guid PlayerId { get; set; }
        public GetPlayerCareerQuery(Guid playerId) => PlayerId = playerId;
    }
}
