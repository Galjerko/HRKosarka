using HRKošarka.Application.Models.Responses;
using MediatR;

namespace HRKošarka.Application.Features.Player.Queries.GetAvailablePlayers
{
    public class GetAvailablePlayersQuery : IRequest<QueryResponse<List<AvailablePlayerDTO>>>
    {
        public Guid TeamId { get; set; }
        public string? SearchTerm { get; set; }
    }
}
