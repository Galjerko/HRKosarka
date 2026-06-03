using HRKošarka.Application.Models.Responses;
using MediatR;

namespace HRKošarka.Application.Features.UserFavoriteTeam.Queries.GetMyFavoriteTeams
{
    public class GetMyFavoriteTeamsQuery : IRequest<QueryResponse<List<FavoriteTeamDTO>>>
    {
        public string UserId { get; set; } = string.Empty;

        public GetMyFavoriteTeamsQuery(string userId) => UserId = userId;
    }
}
