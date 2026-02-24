using HRKošarka.Application.Contracts.Logging;
using HRKošarka.Application.Contracts.Persistence;
using HRKošarka.Application.Exceptions;
using MediatR;

namespace HRKošarka.Application.Features.Season.Commands.DeleteSeason
{
    public class DeleteSeasonCommandHandler : IRequestHandler<DeleteSeasonCommand, Unit>
    {
        private readonly IGenericRepository<Domain.Season> _seasonRepository;
        private readonly IAppLogger<DeleteSeasonCommandHandler> _logger;

        public DeleteSeasonCommandHandler(
            IGenericRepository<Domain.Season> seasonRepository,
            IAppLogger<DeleteSeasonCommandHandler> logger)
        {
            _seasonRepository = seasonRepository;
            _logger = logger;
        }

        public async Task<Unit> Handle(DeleteSeasonCommand request, CancellationToken cancellationToken)
        {
            _logger.LogInformation("Attempting to delete season with ID: {Id}", request.Id);

            var season = await _seasonRepository.GetByIdAsync(request.Id, cancellationToken);

            if (season == null)
            {
                _logger.LogWarning("Season with ID {Id} not found", request.Id);
                throw new NotFoundException(nameof(Domain.Season), request.Id);
            }

            if (season.DateDeleted != null)
            {
                _logger.LogInformation("Season with ID {Id} is already deleted", request.Id);
                throw new BadRequestException("Season is already deleted");
            }

            await _seasonRepository.DeleteAsync(season.Id, cancellationToken);

            _logger.LogInformation("Successfully deleted season with ID: {Id}", request.Id);

            return Unit.Value;
        }
    }
}