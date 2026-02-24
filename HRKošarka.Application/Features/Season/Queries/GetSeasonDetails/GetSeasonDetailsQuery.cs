using HRKošarka.Application.Models.Responses;
using MediatR;

namespace HRKošarka.Application.Features.Season.Queries.GetSeasonDetails
{
    public class GetSeasonDetailsQuery : IRequest<QueryResponse<SeasonDetailsDTO>>
    {
        public Guid Id { get; set; }
        public GetSeasonDetailsQuery(Guid id) => Id = id;
    }
}