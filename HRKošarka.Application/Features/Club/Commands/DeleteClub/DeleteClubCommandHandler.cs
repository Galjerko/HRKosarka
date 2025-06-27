using HRKošarka.Application.Contracts.Logging;
using HRKošarka.Application.Contracts.Persistence;
using HRKošarka.Application.Exceptions;
using MediatR;

namespace HRKošarka.Application.Features.Club.Commands.DeleteClub
{
    public class DeleteClubCommandHandler : IRequestHandler<DeleteClubCommand, Unit>
    {
        private readonly IClubRepository _clubRepository;
        private readonly IAppLogger<DeleteClubCommandHandler> _logger;
        public DeleteClubCommandHandler(IClubRepository clubRepository, IAppLogger<DeleteClubCommandHandler> logger)
        {
            _clubRepository = clubRepository;
            _logger = logger;
        }
        public async Task<Unit> Handle(DeleteClubCommand request, CancellationToken cancellationToken)
        {
            _logger.LogInformation("Attempting to delete club with ID: {Id}", request.Id);

            var clubToDelete = await _clubRepository.GetByIdAsync(request.Id);

            if (clubToDelete == null)
            {
                _logger.LogWarning("Club with ID {Id} not found", request.Id);
                throw new NotFoundException(nameof(Club), request.Id);
            }

            if (clubToDelete.DateDeleted != null)
            {
                _logger.LogInformation("Club with ID {Id} is already deleted", request.Id);
                throw new BadRequestException("Club is already deleted");
            }

            await _clubRepository.DeleteAsync(clubToDelete.Id);

            _logger.LogInformation("Successfully deleted club with ID: {Id}", request.Id);

            return Unit.Value;
        }
    }   
}
