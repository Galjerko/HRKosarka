using HRKošarka.Application.Contracts.Persistence;
using HRKošarka.Application.Models.Responses;
using MediatR;

namespace HRKošarka.Application.Features.UserFavoriteTeam.Queries.GetMyFavoriteTeams
{
    public class GetMyFavoriteTeamsQueryHandler
        : IRequestHandler<GetMyFavoriteTeamsQuery, QueryResponse<List<FavoriteTeamDTO>>>
    {
        private readonly IUserFavoriteTeamRepository _repository;

        public GetMyFavoriteTeamsQueryHandler(IUserFavoriteTeamRepository repository)
        {
            _repository = repository;
        }

        public async Task<QueryResponse<List<FavoriteTeamDTO>>> Handle(
            GetMyFavoriteTeamsQuery request, CancellationToken ct)
        {
            var favorites = await _repository.GetByUserAsync(request.UserId, ct);
            return QueryResponse<List<FavoriteTeamDTO>>.Success(favorites);
        }
    }
}
