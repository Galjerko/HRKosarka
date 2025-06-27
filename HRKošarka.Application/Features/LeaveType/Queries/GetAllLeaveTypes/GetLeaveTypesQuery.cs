using HRKošarka.Application.Models.Requests;
using HRKošarka.Application.Models.Responses;
using MediatR;

namespace HRKošarka.Application.Features.LeaveType.Queries.GetAllLeaveTypes
{
    public class GetLeaveTypesQuery : PaginationRequest, IRequest<PaginatedResponse<LeaveTypeDTO>>
    {
        public GetLeaveTypesQuery()
        {
            SearchableProperties = new List<string> { "Name", "DefaultDays" };
        }
    }
}

