
using HRKošarka.Application.Contracts.Logging;
using HRKošarka.Application.Contracts.Persistence;
using HRKošarka.Application.Exceptions;
using HRKošarka.Application.Features.Club.Commands.DeactivateClub;
using MediatR;

namespace HRKošarka.Application.Features.Club.Commands.ActivateClub
{
    public class ActivateClubCommandHandler : IRequestHandler<ActivateClubCommand, Unit>
    {
        private readonly IClubRepository _clubRepository;
        private IAppLogger<DeactivateClubCommandHandler> _logger;

        public ActivateClubCommandHandler(IClubRepository clubRepository, IAppLogger<DeactivateClubCommandHandler> logger)
        {
            _clubRepository = clubRepository;
            _logger = logger;
        }

        public async Task<Unit> Handle(ActivateClubCommand request, CancellationToken cancellationToken)
        {
            var clubToActivate = await _clubRepository.GetByIdAsync(request.Id);

            if (clubToActivate == null)
            {
                _logger.LogWarning("Club with ID {Id} not found", request.Id);
                throw new NotFoundException(nameof(Domain.Club), request.Id);
            }

            if (clubToActivate.IsActive == true)
            {
                _logger.LogInformation("Club with ID {Id} is already activate", request.Id);
                throw new BadRequestException("Club is already activated");
            };


            clubToActivate.DeactivateDate = null;
            await _clubRepository.UpdateAsync(clubToActivate);

            _logger.LogInformation("Club {ClubName} activated at {ActivateDate}", clubToActivate.Name, DateTime.Now);

            return Unit.Value;
        }
    }
}
