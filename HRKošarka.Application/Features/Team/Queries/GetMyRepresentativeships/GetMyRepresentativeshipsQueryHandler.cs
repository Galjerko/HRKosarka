using HRKošarka.Application.Contracts.Persistence;
using HRKošarka.Application.Models.Responses;
using MediatR;

namespace HRKošarka.Application.Features.Team.Queries.GetMyRepresentativeships
{
    public class GetMyRepresentativeshipsQueryHandler
        : IRequestHandler<GetMyRepresentativeshipsQuery, QueryResponse<List<TeamRepMembershipDTO>>>
    {
        private readonly ITeamRepresentativeRepository _repRepository;

        public GetMyRepresentativeshipsQueryHandler(ITeamRepresentativeRepository repRepository)
        {
            _repRepository = repRepository;
        }

        public async Task<QueryResponse<List<TeamRepMembershipDTO>>> Handle(
            GetMyRepresentativeshipsQuery request, CancellationToken ct)
        {
            var teams = await _repRepository.GetActiveMembershipsByUserAsync(request.UserId, ct);
            return QueryResponse<List<TeamRepMembershipDTO>>.Success(teams);
        }
    }
}
