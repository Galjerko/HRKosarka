using HRKošarka.Application.Models.Responses;
using MediatR;

namespace HRKošarka.Application.Features.UserFavoriteTeam.Commands.ToggleFavoriteTeam
{
    public class ToggleFavoriteTeamCommand : IRequest<CommandResponse<bool>>
    {
        public Guid TeamId { get; set; }
        public string UserId { get; set; } = string.Empty;
    }
}
