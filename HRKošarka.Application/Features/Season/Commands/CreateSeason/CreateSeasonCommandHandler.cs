using AutoMapper;
using HRKošarka.Application.Contracts.Logging;
using HRKošarka.Application.Contracts.Persistence;
using HRKošarka.Application.Exceptions;
using HRKošarka.Application.Models.Responses;
using MediatR;

namespace HRKošarka.Application.Features.Season.Commands.CreateSeason
{
    public class CreateSeasonCommandHandler : IRequestHandler<CreateSeasonCommand, CommandResponse<Guid>>
    {
        private readonly IMapper _mapper;
        private readonly IGenericRepository<Domain.Season> _seasonRepository;
        private readonly IAppLogger<CreateSeasonCommandHandler> _logger;

        public CreateSeasonCommandHandler(
            IMapper mapper,
            IGenericRepository<Domain.Season> seasonRepository,
            IAppLogger<CreateSeasonCommandHandler> logger)
        {
            _mapper = mapper;
            _seasonRepository = seasonRepository;
            _logger = logger;
        }

        public async Task<CommandResponse<Guid>> Handle(
            CreateSeasonCommand request, CancellationToken cancellationToken)
        {
            var validator = new CreateSeasonCommandValidator();
            var validationResult = await validator.ValidateAsync(request, cancellationToken);

            if (!validationResult.IsValid)
            {
                throw new BadRequestException("Invalid Season", validationResult);
            }

            var season = _mapper.Map<Domain.Season>(request);
            await _seasonRepository.CreateAsync(season, cancellationToken);

            _logger.LogInformation("Season {Name} created with Id {Id}", season.Name, season.Id);

            return CommandResponse<Guid>.Success(season.Id, "Season created successfully.");
        }
    }
}