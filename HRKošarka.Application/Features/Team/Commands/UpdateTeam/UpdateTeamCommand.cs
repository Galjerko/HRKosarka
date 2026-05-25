using HRKošarka.Domain.Common;
using MediatR;

namespace HRKošarka.Application.Features.Team.Commands.UpdateTeam
{
    public class UpdateTeamCommand : IRequest<Unit>
    {
        public Guid Id { get; set; }
        public string Name { get; set; } = string.Empty;
        public string? RequesterClubId { get; set; }
        public string? RequesterUserId { get; set; }
    }
}