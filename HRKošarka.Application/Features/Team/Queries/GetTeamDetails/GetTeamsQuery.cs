using HRKošarka.Application.Models.Requests;
using HRKošarka.Application.Models.Responses;
using MediatR;

namespace HRKošarka.Application.Features.Team.Queries.GetAllTeams
{
    public class GetTeamsQuery : PaginationRequest, IRequest<PaginatedResponse<TeamDTO>>
    {
        public GetTeamsQuery()
        {
            SearchableProperties = new List<string> { "Name", "ClubName", "AgeCategoryName" };
            SortableProperties = new List<string> { "Name", "ClubName", "AgeCategoryName", "DateCreated" };
        }
    }
}