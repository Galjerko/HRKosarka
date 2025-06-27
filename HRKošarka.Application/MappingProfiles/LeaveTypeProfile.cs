using AutoMapper;
using HRKošarka.Application.Features.LeaveType.Commands.CreateLeaveType;
using HRKošarka.Application.Features.LeaveType.Commands.UpdateLeaveType;
using HRKošarka.Application.Features.LeaveType.Queries.GetAllLeaveTypes;
using HRKošarka.Application.Features.LeaveType.Queries.GetLeaveTypeDetails;
using HRKošarka.Domain;

namespace HRKošarka.Application.MappingProfiles
{
    public class LeaveTypeProfile : Profile
    {
        public LeaveTypeProfile()
        {
            CreateMap<LeaveTypeDTO, LeaveType>().ReverseMap();
            CreateMap<LeaveType, LeaveTypeDetailsDto>();

            CreateMap<CreateLeaveTypeCommand, LeaveType>();
            CreateMap<UpdateLeaveTypeCommand, LeaveType>();
        }
    }
}
