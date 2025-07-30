using HRKošarka.Application.Models.Responses;
using HRKošarka.Domain.Common;
using MediatR;

namespace HRKošarka.Application.Features.Team.Commands.CreateTeam
{
    public class CreateTeamCommand : IRequest<CommandResponse<Guid>>
    {
        public string Name { get; set; } = string.Empty;
        public Guid ClubId { get; set; }
        public Guid AgeCategoryId { get; set; } 
        public Gender Gender { get; set; }
    }
}
