using HRKošarka.Application.Models.Responses;
using MediatR;

namespace HRKošarka.Application.Features.Team.Queries.GetTeamDetails
{
    public record GetTeamDetailsQuery(Guid Id) : IRequest<QueryResponse<TeamDetailsDTO>>;
}