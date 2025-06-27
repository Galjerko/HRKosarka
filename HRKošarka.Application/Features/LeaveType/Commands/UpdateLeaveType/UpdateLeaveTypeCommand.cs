using MediatR;

namespace HRKošarka.Application.Features.LeaveType.Commands.UpdateLeaveType
{
    public class UpdateLeaveTypeCommand : IRequest<Unit>
    {
        public Guid Id { get; set; } 
        public string Name { get; set; } = string.Empty;
        public int DefaultDays { get; set; }
    }
}
