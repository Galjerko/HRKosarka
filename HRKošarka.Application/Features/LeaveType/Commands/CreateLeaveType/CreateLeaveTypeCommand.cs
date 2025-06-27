using HRKošarka.Application.Models.Responses;
using MediatR;

namespace HRKošarka.Application.Features.LeaveType.Commands.CreateLeaveType
{
    public class CreateLeaveTypeCommand : IRequest<CommandResponse<Guid>>
    {
        public string Name { get; set; } = string.Empty;
        public int DefaultDays { get; set; }
    }
}
