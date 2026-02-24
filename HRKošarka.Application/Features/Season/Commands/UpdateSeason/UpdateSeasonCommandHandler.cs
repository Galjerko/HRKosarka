using AutoMapper;
using HRKošarka.Application.Contracts.Logging;
using HRKošarka.Application.Contracts.Persistence;
using HRKošarka.Application.Exceptions;
using MediatR;

namespace HRKošarka.Application.Features.Season.Commands.UpdateSeason
{
    public class UpdateSeasonCommandHandler : IRequestHandler<UpdateSeasonCommand, Unit>
    {
        private readonly IMapper _mapper;
        private readonly IGenericRepository<Domain.Season> _seasonRepository;
        private readonly IAppLogger<UpdateSeasonCommandHandler> _logger;

        public UpdateSeasonCommandHandler(
            IMapper mapper,
            IGenericRepository<Domain.Season> seasonRepository,
            IAppLogger<UpdateSeasonCommandHandler> logger)
        {
            _mapper = mapper;
            _seasonRepository = seasonRepository;
            _logger = logger;
        }

        public async Task<Unit> Handle(UpdateSeasonCommand request, CancellationToken cancellationToken)
        {
            var validator = new UpdateSeasonCommandValidator();
            var validationResult = await validator.ValidateAsync(request, cancellationToken);

            if (!validationResult.IsValid)
            {
                _logger.LogWarning("Validation errors in update request for {0} - {1}", nameof(Domain.Season), request.Id);
                throw new BadRequestException("Invalid Season", validationResult);
            }

            var season = await _seasonRepository.GetByIdAsync(request.Id, cancellationToken)
                ?? throw new NotFoundException(nameof(Domain.Season), request.Id);

            _mapper.Map(request, season);
            await _seasonRepository.UpdateAsync(season, cancellationToken);

            _logger.LogInformation("Season {Id} updated successfully.", season.Id);

            return Unit.Value;
        }
    }
}