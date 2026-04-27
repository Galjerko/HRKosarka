using AutoMapper;
using HRKošarka.Application.Contracts.Persistence;
using HRKošarka.Application.Exceptions;
using HRKošarka.Application.Models.Responses;
using MediatR;

namespace HRKošarka.Application.Features.Player.Commands.CreatePlayer
{
    public class CreatePlayerCommandHandler : IRequestHandler<CreatePlayerCommand, CommandResponse<Guid>>
    {
        private readonly IMapper _mapper;
        private readonly IPlayerRepository _playerRepository;

        public CreatePlayerCommandHandler(IMapper mapper, IPlayerRepository playerRepository)
        {
            _mapper = mapper;
            _playerRepository = playerRepository;
        }

        public async Task<CommandResponse<Guid>> Handle(CreatePlayerCommand request, CancellationToken cancellationToken)
        {
            var validator = new CreatePlayerCommandValidator(_playerRepository);
            var validationResult = await validator.ValidateAsync(request, cancellationToken);

            if (!validationResult.IsValid)
            {
                throw new BadRequestException("Invalid Player", validationResult);
            }

            var playerToCreate = _mapper.Map<Domain.Player>(request);
            await _playerRepository.CreateAsync(playerToCreate, cancellationToken);

            return CommandResponse<Guid>.Success(playerToCreate.Id, "Player created successfully");
        }
    }
}
