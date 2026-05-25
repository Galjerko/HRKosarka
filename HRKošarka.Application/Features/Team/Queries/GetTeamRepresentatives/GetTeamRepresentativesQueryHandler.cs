using HRKošarka.Application.Contracts.Persistence;
using HRKošarka.Application.Models.Responses;
using MediatR;

namespace HRKošarka.Application.Features.Team.Queries.GetTeamRepresentatives
{
    public class GetTeamRepresentativesQueryHandler
        : IRequestHandler<GetTeamRepresentativesQuery, QueryResponse<List<TeamRepresentativeDTO>>>
    {
        private readonly ITeamRepresentativeRepository _repRepository;

        public GetTeamRepresentativesQueryHandler(ITeamRepresentativeRepository repRepository)
        {
            _repRepository = repRepository;
        }

        public async Task<QueryResponse<List<TeamRepresentativeDTO>>> Handle(
            GetTeamRepresentativesQuery request, CancellationToken ct)
        {
            var reps = await _repRepository.GetByTeamAsync(request.TeamId, ct);
            return QueryResponse<List<TeamRepresentativeDTO>>.Success(reps);
        }
    }
}
