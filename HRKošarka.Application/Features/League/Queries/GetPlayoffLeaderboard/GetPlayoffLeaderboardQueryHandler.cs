using HRKošarka.Application.Contracts.Persistence;
using HRKošarka.Application.Features.League.Queries.GetLeagueLeaderboard;
using HRKošarka.Application.Models.Responses;
using MediatR;

namespace HRKošarka.Application.Features.League.Queries.GetPlayoffLeaderboard
{
    public class GetPlayoffLeaderboardQueryHandler
        : IRequestHandler<GetPlayoffLeaderboardQuery, QueryResponse<List<LeaguePlayerStatDTO>>>
    {
        private readonly IPlayoffRepository _playoffRepository;

        public GetPlayoffLeaderboardQueryHandler(IPlayoffRepository playoffRepository)
            => _playoffRepository = playoffRepository;

        public async Task<QueryResponse<List<LeaguePlayerStatDTO>>> Handle(
            GetPlayoffLeaderboardQuery request, CancellationToken cancellationToken)
        {
            var data = await _playoffRepository.GetPlayoffLeaderboardAsync(
                request.LeagueId, request.SortBy, request.SortDirection, cancellationToken);
            return QueryResponse<List<LeaguePlayerStatDTO>>.Success(data);
        }
    }
}
