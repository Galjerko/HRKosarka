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
        private readonly IAppLogger<CreateLeagueCommandHandler> _logger;

        public CreateLeagueCommandHandler(
            IMapper mapper,
            IGenericRepository<Domain.League> leagueRepository,
            IAppLogger<CreateLeagueCommandHandler> logger)
        {
            _mapper = mapper;
            _leagueRepository = leagueRepository;
            _logger = logger;
        }

        public async Task<CommandResponse<Guid>> Handle(
            CreateLeagueCommand request, CancellationToken cancellationToken)
        {
            var validator = new CreateLeagueCommandValidator();
            var validationResult = await validator.ValidateAsync(request, cancellationToken);

            if (!validationResult.IsValid)
            {
                throw new BadRequestException("Invalid League", validationResult);
            }

            var league = _mapper.Map<Domain.League>(request);
            await _leagueRepository.CreateAsync(league, cancellationToken);

            _logger.LogInformation("League {Name} created with Id {Id}", league.Name, league.Id);

            return CommandResponse<Guid>.Success(league.Id, "League created successfully.");
        }
    }
}
