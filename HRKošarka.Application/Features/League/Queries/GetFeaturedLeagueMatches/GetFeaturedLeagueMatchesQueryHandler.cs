using HRKošarka.Application.Contracts.Persistence;
using HRKošarka.Application.Models.Responses;
using MediatR;

namespace HRKošarka.Application.Features.League.Queries.GetFeaturedLeagueMatches
{
    public class GetFeaturedLeagueMatchesQueryHandler
        : IRequestHandler<GetFeaturedLeagueMatchesQuery, QueryResponse<List<FeaturedLeagueRoundDTO>>>
    {
        private readonly ILeagueRepository _leagueRepository;

        public GetFeaturedLeagueMatchesQueryHandler(ILeagueRepository leagueRepository)
        {
            _leagueRepository = leagueRepository;
        }

        public async Task<QueryResponse<List<FeaturedLeagueRoundDTO>>> Handle(
            GetFeaturedLeagueMatchesQuery request, CancellationToken cancellationToken)
        {
            var data = await _leagueRepository.GetFeaturedLeagueMatchesAsync(cancellationToken);
            return QueryResponse<List<FeaturedLeagueRoundDTO>>.Success(data);
        }
    }
}
