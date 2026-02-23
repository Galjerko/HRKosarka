using HRKošarka.Application.Features.Club.Queries.GetClubsWIthoutManager;
using HRKošarka.Application.Models.Responses;
using MediatR;

namespace HRKošarka.Application.Features.Club.Queries.GetClubsWithoutManager
{
    public class GetClubsWithoutManagerQuery
        : IRequest<QueryResponse<List<ClubWithoutManagerDTO>>>
    {
        public string? SearchTerm { get; set; }
    }
}
