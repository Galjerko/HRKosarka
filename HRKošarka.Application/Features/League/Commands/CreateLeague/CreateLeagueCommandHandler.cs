using AutoMapper;
using HRKošarka.Application.Contracts.Logging;
using HRKošarka.Application.Contracts.Persistence;
using HRKošarka.Application.Exceptions;
using HRKošarka.Application.Models.Responses;
using MediatR;

namespace HRKošarka.Application.Features.League.Commands.CreateLeague
{
    public class CreateLeagueCommandHandler : IRequestHandler<CreateLeagueCommand, CommandResponse<Guid>>
    {
        private readonly IMapper _mapper;
        private readonly IGenericRepository<Domain.League> _leagueRepository;
        private readonly IGenericRepository<Domain.Season> _seasonRepository;
        private readonly IAppLogger<CreateLeagueCommandHandler> _logger;

        public CreateLeagueCommandHandler(
            IMapper mapper,
            IGenericRepository<Domain.League> leagueRepository,
            IGenericRepository<Domain.Season> seasonRepository,
            IAppLogger<CreateLeagueCommandHandler> logger)
        {
            _mapper = mapper;
            _leagueRepository = leagueRepository;
            _seasonRepository = seasonRepository;
            _logger = logger;
        }

        public async Task<CommandResponse<Guid>> Handle(
            CreateLeagueCommand request, CancellationToken cancellationToken)
        {
            var validator = new CreateLeagueCommandValidator();
            var validationResult = await validator.ValidateAsync(request, cancellationToken);

            if (!validationResult.IsValid)
                throw new BadRequestException("Invalid League", validationResult);

            var season = await _seasonRepository.GetByIdAsync(request.SeasonId, cancellationToken)
                ?? throw new NotFoundException(nameof(Domain.Season), request.SeasonId);

            if (request.StartDate.Date < season.StartDate.Date || request.EndDate.Date > season.EndDate.Date)
                throw new BadRequestException(
                    $"League dates must be within the season period ({season.StartDate:dd.MM.yyyy} – {season.EndDate:dd.MM.yyyy}).");

            var league = _mapper.Map<Domain.League>(request);
            league.IsActive = true;
            await _leagueRepository.CreateAsync(league, cancellationToken);

            _logger.LogInformation("League {Name} created with Id {Id}", league.Name, league.Id);

            return CommandResponse<Guid>.Success(league.Id, "League created successfully.");
        }
    }
}
