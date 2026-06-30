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
        private readonly IGenericRepository<Domain.Season> _seasonRepository;
        private readonly IAppLogger<UpdateLeagueCommandHandler> _logger;

        public UpdateLeagueCommandHandler(
            IMapper mapper,
            IGenericRepository<Domain.League> leagueRepository,
            IGenericRepository<Domain.Season> seasonRepository,
            IAppLogger<UpdateLeagueCommandHandler> logger)
        {
            _mapper = mapper;
            _leagueRepository = leagueRepository;
            _seasonRepository = seasonRepository;
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

            var season = await _seasonRepository.GetByIdAsync(request.SeasonId, cancellationToken)
                ?? throw new NotFoundException(nameof(Domain.Season), request.SeasonId);

            if (request.StartDate.Date < season.StartDate.Date || request.EndDate.Date > season.EndDate.Date)
                throw new BadRequestException(
                    $"League dates must be within the season period ({season.StartDate:dd.MM.yyyy} – {season.EndDate:dd.MM.yyyy}).");

            if (league.PlayoffGenerated && league.PlayoffTeamCount != request.PlayoffTeamCount)
                throw new BadRequestException("Cannot change PlayoffTeamCount after playoff has been generated.");

            _mapper.Map(request, league);
            await _leagueRepository.UpdateAsync(league, cancellationToken);

            _logger.LogInformation("League {Id} updated successfully.", league.Id);

            return Unit.Value;
        }
    }
}
