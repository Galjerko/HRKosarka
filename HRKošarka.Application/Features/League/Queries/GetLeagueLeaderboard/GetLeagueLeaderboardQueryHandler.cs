using HRKošarka.Application.Contracts.Persistence;
using HRKošarka.Application.Models.Responses;
using MediatR;

namespace HRKošarka.Application.Features.League.Queries.GetLeagueLeaderboard
{
    public class GetLeagueLeaderboardQueryHandler
        : IRequestHandler<GetLeagueLeaderboardQuery, QueryResponse<List<LeaguePlayerStatDTO>>>
    {
        private readonly ILeagueRepository _leagueRepository;

        public GetLeagueLeaderboardQueryHandler(ILeagueRepository leagueRepository)
            => _leagueRepository = leagueRepository;

        public async Task<QueryResponse<List<LeaguePlayerStatDTO>>> Handle(
            GetLeagueLeaderboardQuery request, CancellationToken cancellationToken)
        {
            var data = await _leagueRepository.GetLeagueLeaderboardAsync(
                request.LeagueId, request.SortBy, request.SortDirection, cancellationToken);
            return QueryResponse<List<LeaguePlayerStatDTO>>.Success(data);
        }
    }
}
