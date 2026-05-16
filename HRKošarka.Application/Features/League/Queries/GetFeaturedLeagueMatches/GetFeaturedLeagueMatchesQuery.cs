using HRKošarka.Application.Models.Responses;
using MediatR;

namespace HRKošarka.Application.Features.League.Queries.GetFeaturedLeagueMatches
{
    public class GetFeaturedLeagueMatchesQuery : IRequest<QueryResponse<List<FeaturedLeagueRoundDTO>>>
    {
    }
}
