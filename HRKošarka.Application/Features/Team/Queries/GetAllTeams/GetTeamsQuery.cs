using HRKošarka.Application.Models.Requests;
using HRKošarka.Application.Models.Responses;
using HRKošarka.Domain.Common;
using MediatR;

namespace HRKošarka.Application.Features.Team.Queries.GetAllTeams
{
    public class GetTeamsQuery : PaginationRequest, IRequest<PaginatedResponse<TeamDTO>>
    {
        public Guid? AgeCategoryId { get; set; }
        public Gender? Gender { get; set; }
        public bool? IsActive { get; set; }

        public GetTeamsQuery()
        {
            // These strings must match property names in your Team entity or your custom projection
            SearchableProperties = new List<string> { "Name", "ClubName", "AgeCategoryName" };
            SortableProperties = new List<string> { "Name", "ClubName", "AgeCategoryName"};
        }
    }
}
