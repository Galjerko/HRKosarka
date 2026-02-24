using AutoMapper;
using HRKošarka.Application.Contracts.Logging;
using HRKošarka.Application.Contracts.Persistence;
using HRKošarka.Application.Exceptions;
using MediatR;

namespace HRKošarka.Application.Features.Club.Commands.UpdateClub
{
    public class UpdateClubCommandHandler : IRequestHandler<UpdateClubCommand, Unit>
    {
        private readonly IMapper _mapper;
        private readonly IClubRepository _clubRepository;
        private readonly IAppLogger<UpdateClubCommandHandler> _logger;

        public UpdateClubCommandHandler(IMapper mapper, IClubRepository clubRepository, IAppLogger<UpdateClubCommandHandler> logger)
        {
            _mapper = mapper;
            _clubRepository = clubRepository;
            _logger = logger;
        }

        public async Task<Unit> Handle(UpdateClubCommand request, CancellationToken cancellationToken)
        {
            var validator = new UpdateClubCommandValidator(_clubRepository);
            var validationResult = await validator.ValidateAsync(request, cancellationToken);

            if (!validationResult.IsValid)
            {
                _logger.LogWarning("Validation errors in update request for {0} - {1}", nameof(Domain.Club), request.Id);
                throw new BadRequestException("Invalid Club", validationResult);
            }

            var clubToUpdate = _mapper.Map<Domain.Club>(request);
            await _clubRepository.UpdateAsync(clubToUpdate, cancellationToken);

            _logger.LogInformation("Club {ClubName} (ID: {ClubId}) successfully updated", clubToUpdate.Name, clubToUpdate.Id);

            return Unit.Value;
        }
    }
}
