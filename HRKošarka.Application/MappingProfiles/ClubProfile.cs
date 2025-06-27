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
            CreateMap<Club, ClubDetailsDTO>();

            CreateMap<CreateClubCommand, Club>();
            CreateMap<UpdateClubCommand, Club>();
        }
    }
}
