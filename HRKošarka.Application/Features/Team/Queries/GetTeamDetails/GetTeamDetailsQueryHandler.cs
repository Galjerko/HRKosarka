using AutoMapper;
using HRKošarka.Application.Contracts.Persistence;
using HRKošarka.Application.Exceptions;
using HRKošarka.Application.Models.Responses;
using MediatR;

namespace HRKošarka.Application.Features.Team.Queries.GetTeamDetails
{
    public class GetTeamDetailsQueryHandler : IRequestHandler<GetTeamDetailsQuery, QueryResponse<TeamDetailsDTO>>
    {
        private readonly IMapper _mapper;
        private readonly ITeamRepository _teamRepository;

        public GetTeamDetailsQueryHandler(IMapper mapper, ITeamRepository teamRepository)
        {
            _mapper = mapper;
            _teamRepository = teamRepository;
        }

        public async Task<QueryResponse<TeamDetailsDTO>> Handle(GetTeamDetailsQuery request, CancellationToken cancellationToken)
        {
            var team = await _teamRepository.GetByIdWithIncludesAsync(request.Id);

            if (team == null)
            {
                throw new NotFoundException(nameof(Team), request.Id);
            }

            var data = _mapper.Map<TeamDetailsDTO>(team);

            return QueryResponse<TeamDetailsDTO>.Success(data);
        }
    }
}