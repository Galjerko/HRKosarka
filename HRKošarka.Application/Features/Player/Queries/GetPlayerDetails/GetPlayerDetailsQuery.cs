using HRKošarka.Application.Models.Responses;
using MediatR;

namespace HRKošarka.Application.Features.Player.Queries.GetPlayerDetails
{
    public record GetPlayerDetailsQuery(Guid Id) : IRequest<QueryResponse<PlayerDetailsDTO>>;
}
