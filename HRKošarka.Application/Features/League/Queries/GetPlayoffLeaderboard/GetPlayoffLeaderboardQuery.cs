using HRKošarka.Application.Features.League.Queries.GetLeagueLeaderboard;
using HRKošarka.Application.Models.Responses;
using MediatR;

namespace HRKošarka.Application.Features.League.Queries.GetPlayoffLeaderboard
{
    public class GetPlayoffLeaderboardQuery : IRequest<QueryResponse<List<LeaguePlayerStatDTO>>>
    {
        public Guid LeagueId { get; set; }
        public string? SortBy { get; set; }
        public string? SortDirection { get; set; }

        public GetPlayoffLeaderboardQuery(Guid leagueId, string? sortBy = null, string? sortDirection = null)
        {
            LeagueId = leagueId;
            SortBy = sortBy;
            SortDirection = sortDirection;
        }
    }
}
