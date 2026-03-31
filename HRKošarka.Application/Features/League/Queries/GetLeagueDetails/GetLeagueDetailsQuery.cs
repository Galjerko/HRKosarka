using HRKošarka.Application.Models.Responses;
using MediatR;

namespace HRKošarka.Application.Features.League.Queries.GetLeagueDetails
{
    public record GetLeagueDetailsQuery(Guid Id) : IRequest<QueryResponse<LeagueDetailsDTO>>;
}
