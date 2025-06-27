using AutoMapper;
using HRKošarka.Application.Contracts.Persistence;
using HRKošarka.Application.Exceptions;
using HRKošarka.Application.Models.Responses;
using MediatR;

namespace HRKošarka.Application.Features.LeaveType.Commands.CreateLeaveType
{
    public class CreateLeaveTypeCommandHandler : IRequestHandler<CreateLeaveTypeCommand, CommandResponse<Guid>>
    {
        private readonly IMapper _mapper;
        private readonly ILeaveTypeRepository _leaveTypeRepository;

        public CreateLeaveTypeCommandHandler(IMapper mapper, ILeaveTypeRepository leaveTypeRepository)
        {
            _mapper = mapper;
            _leaveTypeRepository = leaveTypeRepository;
        }

        public async Task<CommandResponse<Guid>> Handle(CreateLeaveTypeCommand request, CancellationToken cancellationToken)
        {
            var validator = new CreateLeaveTypeCommandValidator(_leaveTypeRepository);
            var validationResult = await validator.ValidateAsync(request);

            if (!validationResult.IsValid)
            {
                throw new BadRequestException("Invalid LeaveType", validationResult);
            }

            // Your existing business logic
            var leaveTypeToCreate = _mapper.Map<Domain.LeaveType>(request);
            await _leaveTypeRepository.CreateAsync(leaveTypeToCreate);

            // Return CommandResponse for success
            return CommandResponse<Guid>.Success(
                leaveTypeToCreate.Id,
                "Leave type created successfully"
            );
        }
    }

}
