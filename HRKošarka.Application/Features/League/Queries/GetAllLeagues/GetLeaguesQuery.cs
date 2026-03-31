using HRKošarka.Application.Models.Requests;
using HRKošarka.Application.Models.Responses;
using HRKošarka.Domain.Common;
using MediatR;
using DomainLeague = HRKošarka.Domain.League;

namespace HRKošarka.Application.Features.League.Queries.GetAllLeagues
{
    public class GetLeaguesQuery : PaginationRequest, IRequest<PaginatedResponse<LeagueDTO>>
    {
        public GetLeaguesQuery()
        {
            SearchableProperties = new List<string> { nameof(DomainLeague.Name) };
            SortableProperties = new List<string>
            {
                nameof(DomainLeague.Name),
                nameof(DomainLeague.StartDate),
                nameof(DomainLeague.EndDate),
                nameof(DomainLeague.DateCreated)
            };
        }

        public Guid? SeasonId { get; set; }
        public Guid? AgeCategoryId { get; set; }
        public Gender? Gender { get; set; }
        public CompetitionType? CompetitionType { get; set; }
        public bool? IsActive { get; set; }
    }
}
