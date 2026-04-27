using AutoMapper;
using HRKošarka.Application.Contracts.Persistence;
using HRKošarka.Application.Models.Responses;
using MediatR;

namespace HRKošarka.Application.Features.Team.Queries.GetTeamRoster
{
    public class GetTeamRosterQueryHandler
        : IRequestHandler<GetTeamRosterQuery, QueryResponse<List<TeamRosterPlayerDTO>>>
    {
        private readonly IMapper _mapper;
        private readonly ITeamRepository _teamRepository;

        public GetTeamRosterQueryHandler(IMapper mapper, ITeamRepository teamRepository)
        {
            _mapper = mapper;
            _teamRepository = teamRepository;
        }

        public async Task<QueryResponse<List<TeamRosterPlayerDTO>>> Handle(
            GetTeamRosterQuery request, CancellationToken cancellationToken)
        {
            var roster = await _teamRepository.GetTeamRosterAsync(request.TeamId, cancellationToken);
            var data = _mapper.Map<List<TeamRosterPlayerDTO>>(roster);
            return QueryResponse<List<TeamRosterPlayerDTO>>.Success(data);
        }
    }
}
