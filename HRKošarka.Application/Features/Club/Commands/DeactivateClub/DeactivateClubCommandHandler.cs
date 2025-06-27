using HRKošarka.Application.Contracts.Logging;
using HRKošarka.Application.Contracts.Persistence;
using HRKošarka.Application.Exceptions;
using MediatR;

namespace HRKošarka.Application.Features.Club.Commands.DeactivateClub
{
    public class DeactivateClubCommandHandler : IRequestHandler<DeactivateClubCommand, Unit>
    {
        private readonly IClubRepository _clubRepository;
        private IAppLogger<DeactivateClubCommandHandler> _logger;
        public DeactivateClubCommandHandler(IClubRepository clubRepository, IAppLogger<DeactivateClubCommandHandler> logger)
        {
            _clubRepository = clubRepository;
            _logger = logger;
        }
        public async Task<Unit> Handle(DeactivateClubCommand request, CancellationToken cancellationToken)
        {
            var clubToDeactivate = await _clubRepository.GetByIdAsync(request.Id);

            if (clubToDeactivate == null)
            {
                _logger.LogWarning("Club with ID {Id} not found", request.Id);
                throw new NotFoundException(nameof(Domain.Club), request.Id);
            }

            if (clubToDeactivate.IsActive == false)
            {
                _logger.LogInformation("Club with ID {Id} is already deactivated", request.Id);
                throw new BadRequestException("Club is already deactivated");
            }

            clubToDeactivate.DeactivateDate = DateTime.Now;
            await _clubRepository.UpdateAsync(clubToDeactivate);

            _logger.LogInformation("Club {ClubName} deactivated at {DeactivateDate}", clubToDeactivate.Name, clubToDeactivate.DeactivateDate);

            return Unit.Value;
        }
    }
}
