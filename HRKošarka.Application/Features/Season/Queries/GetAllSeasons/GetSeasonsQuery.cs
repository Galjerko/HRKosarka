using HRKošarka.Application.Models.Requests;
using HRKošarka.Application.Models.Responses;
using MediatR;

namespace HRKošarka.Application.Features.Season.Queries.GetAllSeasons
{
    public class GetSeasonsQuery : PaginationRequest, IRequest<PaginatedResponse<SeasonDTO>>
    {
        public GetSeasonsQuery()
        {
            SearchableProperties = new List<string> { "Name" };
            SortableProperties = new List<string> { "Name", "StartDate", "EndDate", "DateCreated" };
        }
    }
}