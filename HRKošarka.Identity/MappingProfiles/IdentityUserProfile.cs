using AutoMapper;
using HRKošarka.Application.Features.User.Queries.GetNonAdminUsers;
using HRKošarka.Identity.Models;

namespace HRKošarka.Identity.MappingProfiles
{
    public class IdentityUserProfile : Profile
    {
        public IdentityUserProfile()
        {
            CreateMap<ApplicationUser, NonAdminUserDTO>()
                .ForMember(d => d.FullName,
                    opt => opt.MapFrom(s => (s.FirstName + " " + s.LastName).Trim()))
                .ForMember(d => d.Role, opt => opt.Ignore())
                .ForMember(d => d.ManagedClubId,
                    opt => opt.MapFrom(s => s.ManagedClubId))
                .ForMember(d => d.ManagedClubName, opt => opt.Ignore());
        }
    }
}
