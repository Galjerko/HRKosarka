using HRKošarka.Application.Contracts.Persistence;
using HRKošarka.Application.Features.League.Queries.GetLeagueStandings;
using HRKošarka.Application.Models.Responses;
using MediatR;

namespace HRKošarka.Application.Features.League.Queries.GetPlayoffLeaders
{
    public class GetPlayoffLeadersQueryHandler
        : IRequestHandler<GetPlayoffLeadersQuery, QueryResponse<LeagueLeadersDTO?>>
    {
        private readonly IPlayoffRepository _playoffRepository;

        public GetPlayoffLeadersQueryHandler(IPlayoffRepository playoffRepository)
        {
            _playoffRepository = playoffRepository;
        }

        public async Task<QueryResponse<LeagueLeadersDTO?>> Handle(
            GetPlayoffLeadersQuery request, CancellationToken ct)
        {
            var leaders = await _playoffRepository.GetPlayoffLeadersAsync(request.LeagueId, ct);
            return QueryResponse<LeagueLeadersDTO?>.Success(leaders.TopScorers.Any() ? leaders : null);
        }
    }
}
