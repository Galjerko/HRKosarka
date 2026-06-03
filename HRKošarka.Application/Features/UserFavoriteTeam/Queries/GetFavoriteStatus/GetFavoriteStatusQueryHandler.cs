using HRKošarka.Application.Contracts.Persistence;
using HRKošarka.Application.Models.Responses;
using MediatR;

namespace HRKošarka.Application.Features.UserFavoriteTeam.Queries.GetFavoriteStatus
{
    public class GetFavoriteStatusQueryHandler
        : IRequestHandler<GetFavoriteStatusQuery, QueryResponse<bool>>
    {
        private readonly IUserFavoriteTeamRepository _repository;

        public GetFavoriteStatusQueryHandler(IUserFavoriteTeamRepository repository)
        {
            _repository = repository;
        }

        public async Task<QueryResponse<bool>> Handle(
            GetFavoriteStatusQuery request, CancellationToken ct)
        {
            var isFavorited = await _repository.IsFavoritedAsync(request.UserId, request.TeamId, ct);
            return QueryResponse<bool>.Success(isFavorited);
        }
    }
}
