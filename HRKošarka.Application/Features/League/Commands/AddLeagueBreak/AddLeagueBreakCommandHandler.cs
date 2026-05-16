using HRKošarka.Application.Contracts.Logging;
using HRKošarka.Application.Contracts.Persistence;
using HRKošarka.Application.Exceptions;
using HRKošarka.Application.Models.Responses;
using HRKošarka.Domain;
using MediatR;

namespace HRKošarka.Application.Features.League.Commands.AddLeagueBreak
{
    public class AddLeagueBreakCommandHandler : IRequestHandler<AddLeagueBreakCommand, CommandResponse<Guid>>
    {
        private readonly ILeagueRepository _leagueRepository;
        private readonly IGenericRepository<LeagueBreak> _breakRepository;
        private readonly IAppLogger<AddLeagueBreakCommandHandler> _logger;

        public AddLeagueBreakCommandHandler(
            ILeagueRepository leagueRepository,
            IGenericRepository<LeagueBreak> breakRepository,
            IAppLogger<AddLeagueBreakCommandHandler> logger)
        {
            _leagueRepository = leagueRepository;
            _breakRepository = breakRepository;
            _logger = logger;
        }

        public async Task<CommandResponse<Guid>> Handle(AddLeagueBreakCommand request, CancellationToken cancellationToken)
        {
            _logger.LogInformation("Attempting to add break '{Name}' to league {LeagueId}", request.Name, request.LeagueId);

            var validator = new AddLeagueBreakCommandValidator();
            var validationResult = await validator.ValidateAsync(request, cancellationToken);
            if (!validationResult.IsValid)
                throw new BadRequestException("Invalid break data", validationResult);

            var league = await _leagueRepository.GetByIdAsync(request.LeagueId, cancellationToken)
                ?? throw new NotFoundException(nameof(Domain.League), request.LeagueId);

            if (league.ScheduleGenerated)
                throw new BadRequestException("Cannot add breaks after the schedule has been generated.");

            if (request.StartDate < league.StartDate || request.EndDate > league.EndDate)
                throw new BadRequestException("Break dates must fall within the league period.");

            var leagueBreak = new LeagueBreak
            {
                LeagueId = request.LeagueId,
                Name = request.Name,
                StartDate = request.StartDate,
                EndDate = request.EndDate
            };

            await _breakRepository.CreateAsync(leagueBreak, cancellationToken);

            _logger.LogInformation("Break '{Name}' added to league {LeagueId}", request.Name, request.LeagueId);

            return CommandResponse<Guid>.Success(leagueBreak.Id, "Break added successfully.");
        }
    }
}
