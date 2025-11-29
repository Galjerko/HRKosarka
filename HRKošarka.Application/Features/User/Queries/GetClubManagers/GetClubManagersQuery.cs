using HRKošarka.Application.Models.Requests;
using HRKošarka.Application.Models.Responses;
using MediatR;

namespace HRKošarka.Application.Features.User.Queries.GetClubManagers
{
    public class GetClubManagersQuery : PaginationRequest, IRequest<PaginatedResponse<ClubManagerDTO>>
    {
        public GetClubManagersQuery()
        {
            SearchableProperties = new List<string> { "UserName", "Email", "FirstName", "LastName" };
            SortableProperties = new List<string> { "UserName", "Email", "DateCreated" };
        }
    }
}
