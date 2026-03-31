using AutoMapper;
using HRKošarka.Application.Contracts.Logging;
using HRKošarka.Application.Contracts.Persistence;
using HRKošarka.Application.Exceptions;
using MediatR;

namespace HRKošarka.Application.Features.League.Commands.UpdateLeague
{
    public class UpdateLeagueCommandHandler : IRequestHandler<UpdateLeagueCommand, Unit>
    {
        private readonly IMapper _mapper;
        private readonly IGenericRepository<Domain.League> _leagueRepository;
        private readonly IAppLogger<UpdateLeagueCommandHandler> _logger;

        public UpdateLeagueCommandHandler(
            IMapper mapper,
            IGenericRepository<Domain.League> leagueRepository,
            IAppLogger<UpdateLeagueCommandHandler> logger)
        {
            _mapper = mapper;
            _leagueRepository = leagueRepository;
            _logger = logger;
        }

        public async Task<Unit> Handle(UpdateLeagueCommand request, CancellationToken cancellationToken)
        {
            var validator = new UpdateLeagueCommandValidator();
            var validationResult = await validator.ValidateAsync(request, cancellationToken);

            if (!validationResult.IsValid)
            {
                _logger.LogWarning("Validation errors in update request for {0} - {1}",
                    nameof(Domain.League), request.Id);
                throw new BadRequestException("Invalid League", validationResult);
            }

            var league = await _leagueRepository.GetByIdAsync(request.Id, cancellationToken)
                ?? throw new NotFoundException(nameof(Domain.League), request.Id);

            _mapper.Map(request, league);
            await _leagueRepository.UpdateAsync(league, cancellationToken);

            _logger.LogInformation("League {Id} updated successfully.", league.Id);

            return Unit.Value;
        }
    }
}
