using AutoMapper;
using HRKošarka.Application.Contracts.Persistence;
using HRKošarka.Application.Exceptions;
using HRKošarka.Application.Models.Responses;
using MediatR;

namespace HRKošarka.Application.Features.Club.Commands.CreateClub
{
    public class CreateClubCommandHandler : IRequestHandler<CreateClubCommand, CommandResponse<Guid>>
    {
        private readonly IMapper _mapper;
        private readonly IClubRepository _clubRepository;

        public CreateClubCommandHandler(IMapper mapper, IClubRepository clubRepository)
        {
            _mapper = mapper;
            _clubRepository = clubRepository;
        }

        public async Task<CommandResponse<Guid>> Handle(CreateClubCommand request, CancellationToken cancellationToken)
        {
            var validator = new CreateClubCommandValidator(_clubRepository);
            var validationResult = await validator.ValidateAsync(request);

            if (!validationResult.IsValid)
            {
                throw new BadRequestException("Invalid Club", validationResult);
            }

            var clubToCreate = _mapper.Map<Domain.Club>(request);
            await _clubRepository.CreateAsync(clubToCreate);

            return CommandResponse<Guid>.Success(
                clubToCreate.Id,
                "Club created successfully"
            );
        }
    }
}
