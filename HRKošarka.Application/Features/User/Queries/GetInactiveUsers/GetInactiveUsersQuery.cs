using HRKošarka.Application.Models.Requests;
using HRKošarka.Application.Models.Responses;
using MediatR;

namespace HRKošarka.Application.Features.User.Queries.GetInactiveUsers
{
    public class GetInactiveUsersQuery : PaginationRequest, IRequest<PaginatedResponse<InactiveUserDTO>>
    {
        public GetInactiveUsersQuery()
        {
            SearchableProperties = new List<string> { "Email", "FullName" };
        }
    }
}
