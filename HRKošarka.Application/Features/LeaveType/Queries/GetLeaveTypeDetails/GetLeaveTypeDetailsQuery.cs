using MediatR;

namespace HRKošarka.Application.Features.LeaveType.Queries.GetLeaveTypeDetails
{
    public record GetLeaveTypeDetailsQuery(Guid Id) : IRequest<LeaveTypeDetailsDto>;
}
