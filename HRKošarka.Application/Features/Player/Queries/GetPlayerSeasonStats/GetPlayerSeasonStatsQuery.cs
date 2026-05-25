using HRKošarka.Application.Models.Responses;
using MediatR;

namespace HRKošarka.Application.Features.Player.Queries.GetPlayerSeasonStats
{
    public class GetPlayerSeasonStatsQuery : IRequest<QueryResponse<List<PlayerSeasonGroupDTO>>>
    {
        public Guid PlayerId { get; set; }

        public GetPlayerSeasonStatsQuery(Guid playerId) => PlayerId = playerId;
    }
}
