using HRKošarka.Application.Models.Responses;
using MediatR;

namespace HRKošarka.Application.Features.Match.Queries.GetMatchWithStats
{
    public class GetMatchWithStatsQuery : IRequest<QueryResponse<MatchWithStatsDTO>>
    {
        public Guid Id { get; set; }
        public GetMatchWithStatsQuery(Guid id) => Id = id;
    }
}
