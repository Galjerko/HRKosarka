using AutoMapper;
using HRKošarka.Application.Contracts.Persistence;
using HRKošarka.Application.Exceptions;
using HRKošarka.Application.Models.Responses;
using MediatR;

namespace HRKošarka.Application.Features.Team.Commands.CreateTeam
{
    public class CreateTeamCommandHandler : IRequestHandler<CreateTeamCommand, CommandResponse<Guid>>
    {
        private readonly IMapper _mapper;
        private readonly ITeamRepository _teamRepository;

        public CreateTeamCommandHandler(IMapper mapper, ITeamRepository teamRepository)
        {
            _mapper = mapper;
            _teamRepository = teamRepository;
        }

        public async Task<CommandResponse<Guid>> Handle(CreateTeamCommand request, CancellationToken cancellationToken)
        {
            var validator = new CreateTeamCommandValidator(_teamRepository);
            var validationResult = await validator.ValidateAsync(request);

            if (!validationResult.IsValid)
            {
                throw new BadRequestException("Invalid Team", validationResult);
            }

            var teamToCreate = _mapper.Map<Domain.Team>(request);
            await _teamRepository.CreateAsync(teamToCreate);

            return CommandResponse<Guid>.Success(
                teamToCreate.Id,
                "Team created successfully"
            );
        }
    }
}
