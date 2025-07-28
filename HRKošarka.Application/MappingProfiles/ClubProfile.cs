using AutoMapper;
using HRKošarka.Application.Features.Club.Commands.CreateClub;
using HRKošarka.Application.Features.Club.Commands.UpdateClub;
using HRKošarka.Application.Features.Club.Queries.GetAllClubs;
using HRKošarka.Application.Features.Club.Queries.GetClubDetails;
using HRKošarka.Domain;

namespace HRKošarka.Application.MappingProfiles
{
    public class ClubProfile : Profile
    {
        public ClubProfile()
        {
            CreateMap<ClubDTO, Club>().ReverseMap();

            CreateMap<Club, ClubDetailsDTO>()
                .ForMember(dest => dest.FoundedYear, opt => opt.MapFrom(src => src.FoundedYear.Year));

            CreateMap<ClubDetailsDTO, Club>()
                .ForMember(dest => dest.FoundedYear, opt => opt.MapFrom(src => new DateTime(src.FoundedYear, 1, 1)));

            CreateMap<CreateClubCommand, Club>();
            CreateMap<UpdateClubCommand, Club>();
        }
    }
}
