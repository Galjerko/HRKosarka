using HRKošarka.Application.Contracts.Persistence;
using HRKošarka.Application.Models.Responses;
using MediatR;

namespace HRKošarka.Application.Features.League.Queries.GetLeagueBreaks
{
    public class GetLeagueBreaksQueryHandler : IRequestHandler<GetLeagueBreaksQuery, QueryResponse<List<LeagueBreakDTO>>>
    {
        private readonly ILeagueRepository _leagueRepository;

        public GetLeagueBreaksQueryHandler(ILeagueRepository leagueRepository)
        {
            _leagueRepository = leagueRepository;
        }

        public async Task<QueryResponse<List<LeagueBreakDTO>>> Handle(GetLeagueBreaksQuery request, CancellationToken cancellationToken)
        {
            var breaks = await _leagueRepository.GetLeagueBreaksAsync(request.LeagueId, cancellationToken);
            return QueryResponse<List<LeagueBreakDTO>>.Success(breaks);
        }
    }
}
