using HRKošarka.Application.Models.Requests;
using HRKošarka.Application.Models.Responses;
using MediatR;

namespace HRKošarka.Application.Features.Player.Queries.GetAllPlayers
{
    public class GetPlayersQuery : PaginationRequest, IRequest<PaginatedResponse<PlayerDTO>>
    {
        public GetPlayersQuery()
        {
            SearchableProperties = new List<string> { "FirstName", "LastName", "RegistrationNumber" };
            SortableProperties = new List<string> { "FirstName", "LastName", "DateOfBirth", "DateCreated" };
        }
    }
}
