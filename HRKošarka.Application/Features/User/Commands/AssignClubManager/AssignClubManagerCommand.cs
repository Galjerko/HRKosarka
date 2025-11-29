using MediatR;

namespace HRKošarka.Application.Features.User.Commands.AssignClubManager
{
    public class AssignClubManagerCommand : IRequest<Unit>
    {
        public Guid ClubId { get; set; }
        public string UserId { get; set; } = string.Empty;
    }
}
