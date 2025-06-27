using HRKošarka.Application.Models.Responses;
using MediatR;

namespace HRKošarka.Application.Features.Club.Queries.GetClubDetails
{
    public record GetClubDetailsQuery(Guid Id) : IRequest<QueryResponse<ClubDetailsDTO>>;
}
