using HRKošarka.Application.Models.Requests;
using HRKošarka.Application.Models.Responses;
using MediatR;

namespace HRKošarka.Application.Features.Club.Queries.GetAllClubs
{
    public class GetClubsQuery : PaginationRequest, IRequest<PaginatedResponse<ClubDTO>>
    {
        public GetClubsQuery()
        {
            SearchableProperties = new List<string> { "Name",  "City",};
            SortableProperties = new List<string> { "Name", "City", "FoundedYear", "DateCreated" };
        }
    }
}
