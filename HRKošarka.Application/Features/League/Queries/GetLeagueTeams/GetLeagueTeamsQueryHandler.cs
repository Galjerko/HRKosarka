using HRKošarka.Application.Contracts.Persistence;
using HRKošarka.Application.Models.Responses;
using MediatR;

namespace HRKošarka.Application.Features.League.Queries.GetLeagueTeams
{
    public class GetLeagueTeamsQueryHandler : IRequestHandler<GetLeagueTeamsQuery, QueryResponse<List<LeagueTeamDTO>>>
    {
        private readonly ILeagueRepository _leagueRepository;

        public GetLeagueTeamsQueryHandler(ILeagueRepository leagueRepository)
        {
            _leagueRepository = leagueRepository;
        }

        public async Task<QueryResponse<List<LeagueTeamDTO>>> Handle(GetLeagueTeamsQuery request, CancellationToken cancellationToken)
        {
            var teams = await _leagueRepository.GetLeagueTeamsAsync(request.LeagueId, cancellationToken);
            return QueryResponse<List<LeagueTeamDTO>>.Success(teams, "League teams retrieved successfully.");
        }
    }
}
