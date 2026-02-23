using HRKošarka.Application.Models.Requests;
using HRKošarka.Application.Models.Responses;
using MediatR;

namespace HRKošarka.Application.Features.User.Queries.GetNonAdminUsers
{
    public class GetNonAdminUsersQuery : PaginationRequest, IRequest<PaginatedResponse<NonAdminUserDTO>>
    {
        public GetNonAdminUsersQuery()
        {
            SearchableProperties = new List<string> { "Email", "FullName" };
        }
    }
}
