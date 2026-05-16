using HRKošarka.Application.Contracts.Persistence;
using HRKošarka.Application.Models.Responses;
using MediatR;

namespace HRKošarka.Application.Features.League.Queries.GetLeagueSchedule
{
    public class GetLeagueScheduleQueryHandler : IRequestHandler<GetLeagueScheduleQuery, QueryResponse<List<LeagueRoundDTO>>>
    {
        private readonly ILeagueRepository _leagueRepository;

        public GetLeagueScheduleQueryHandler(ILeagueRepository leagueRepository)
        {
            _leagueRepository = leagueRepository;
        }

        public async Task<QueryResponse<List<LeagueRoundDTO>>> Handle(GetLeagueScheduleQuery request, CancellationToken cancellationToken)
        {
            var rounds = await _leagueRepository.GetLeagueScheduleAsync(request.LeagueId, cancellationToken);
            return QueryResponse<List<LeagueRoundDTO>>.Success(rounds);
        }
    }
}
