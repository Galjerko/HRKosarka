using AutoMapper;
using HRKošarka.Application.Contracts.Persistence;
using HRKošarka.Application.Exceptions;
using HRKošarka.Application.Features.Club.Queries.GetClubDetails;
using HRKošarka.Application.Models.Responses;
using MediatR;

namespace HRKošarka.Application.Features.Club.Queries.GetClubDetails
{
    public class GetClubDetailsQueryHandler : IRequestHandler<GetClubDetailsQuery, QueryResponse<ClubDetailsDTO>>
    {
        private readonly IMapper _mapper;
        private readonly IClubRepository _clubRepository;

        public GetClubDetailsQueryHandler(IMapper mapper, IClubRepository clubRepository)
        {
            _mapper = mapper;
            _clubRepository = clubRepository;
        }

        public async Task<QueryResponse<ClubDetailsDTO>> Handle(GetClubDetailsQuery request, CancellationToken cancellationToken)
        {
            var club = await _clubRepository.GetByIdAsync(request.Id);

            if (club == null)
            {
                throw new NotFoundException(nameof(Club), request.Id);
            }

            var data = _mapper.Map<ClubDetailsDTO>(club);

            return QueryResponse<ClubDetailsDTO>.Success(data);
        }
    }
}
