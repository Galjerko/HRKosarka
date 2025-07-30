using AutoMapper;
using HRKošarka.Application.Contracts.Persistence;
using HRKošarka.Application.Exceptions;
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
            var club = await _clubRepository.GetClubWithTeamsAsync(request.Id);

            if (club == null)
            {
                throw new NotFoundException(nameof(Club), request.Id);
            }

            var clubDetailsDto = _mapper.Map<ClubDetailsDTO>(club);

            clubDetailsDto.Teams = club.Teams
                .OrderBy(t => t.Gender) 
                .ThenByDescending(t => t.AgeCategory.Name) 
                .Select(t => new TeamInfoDTO
                {
                    Id = t.Id,
                    Name = t.Name,
                    Gender = t.Gender,
                    AgeCategoryId = t.AgeCategoryId,
                    AgeCategoryName = t.AgeCategory.Name,
                    AgeCategoryCode = t.AgeCategory.Code,
                    IsActive = t.IsActive
                })
                .ToList();

            return QueryResponse<ClubDetailsDTO>.Success(clubDetailsDto);
        }
    }
}
