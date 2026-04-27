using HRKošarka.Application.Models.Responses;
using MediatR;

namespace HRKošarka.Application.Features.Team.Queries.GetTeamRoster
{
    public record GetTeamRosterQuery(Guid TeamId) : IRequest<QueryResponse<List<TeamRosterPlayerDTO>>>;
}
