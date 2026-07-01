using HRKošarka.Application.Features.Player.Queries.GetPlayerSeasonStats;
using HRKošarka.Application.Models.Responses;
using MediatR;

namespace HRKošarka.Application.Features.Player.Queries.GetPlayerPlayoffStats
{
    public class GetPlayerPlayoffStatsQuery : IRequest<QueryResponse<List<PlayerSeasonGroupDTO>>>
    {
        public Guid PlayerId { get; set; }

        public GetPlayerPlayoffStatsQuery(Guid playerId) => PlayerId = playerId;
    }
}
