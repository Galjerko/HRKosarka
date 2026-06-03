using HRKošarka.Application.Models.Responses;
using MediatR;

namespace HRKošarka.Application.Features.UserFavoriteTeam.Queries.GetFavoriteStatus
{
    public class GetFavoriteStatusQuery : IRequest<QueryResponse<bool>>
    {
        public Guid TeamId { get; set; }
        public string UserId { get; set; } = string.Empty;

        public GetFavoriteStatusQuery(Guid teamId, string userId)
        {
            TeamId = teamId;
            UserId = userId;
        }
    }
}
