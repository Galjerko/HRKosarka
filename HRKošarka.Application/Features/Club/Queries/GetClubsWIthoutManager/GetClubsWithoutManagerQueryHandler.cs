using HRKošarka.Application.Contracts.Identity;
using HRKošarka.Application.Contracts.Persistence;
using HRKošarka.Application.Features.Club.Queries.GetClubsWIthoutManager;
using HRKošarka.Application.Models.Responses;
using MediatR;

namespace HRKošarka.Application.Features.Club.Queries.GetClubsWithoutManager
{
    public class GetClubsWithoutManagerQueryHandler
        : IRequestHandler<GetClubsWithoutManagerQuery, QueryResponse<List<ClubWithoutManagerDTO>>>
    {
        private readonly IClubRepository _clubRepository;
        private readonly IUserReadService _userReadService;

        public GetClubsWithoutManagerQueryHandler(
            IClubRepository clubRepository,
            IUserReadService userReadService)
        {
            _clubRepository = clubRepository;
            _userReadService = userReadService;
        }

        public async Task<QueryResponse<List<ClubWithoutManagerDTO>>> Handle(
            GetClubsWithoutManagerQuery request,
            CancellationToken cancellationToken)
        {
            try
            {
                var managedClubIds = await _userReadService.GetManagedClubIdsAsync(cancellationToken);

                var clubs = await _clubRepository.GetClubsWithoutManagerAsync(
                    managedClubIds,
                    request.SearchTerm,
                    cancellationToken);

                return new QueryResponse<List<ClubWithoutManagerDTO>>
                {
                    IsSuccess = true,
                    Data = clubs
                };
            }
            catch (Exception ex)
            {
                return new QueryResponse<List<ClubWithoutManagerDTO>>
                {
                    IsSuccess = false,
                    Data = new List<ClubWithoutManagerDTO>(),
                    Message = "Failed to load clubs without manager",
                    Errors = new List<string> { ex.Message }
                };
            }
        }
    }
}
