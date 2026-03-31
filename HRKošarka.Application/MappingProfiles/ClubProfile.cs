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
                .ForMember(dest => dest.FoundedYear, opt => opt.MapFrom(src => src.FoundedYear.Year))
                .ForMember(dest => dest.Teams, opt => opt.Ignore());

            CreateMap<Domain.League, Features.League.Queries.GetAllLeagues.LeagueDTO>()
                .ForMember(dest => dest.SeasonName, opt => opt.MapFrom(src => src.Season != null ? src.Season.Name : string.Empty))
                .ForMember(dest => dest.AgeCategoryCode, opt => opt.MapFrom(src => src.AgeCategory != null ? src.AgeCategory.Code : string.Empty));

            CreateMap<ClubDetailsDTO, Club>()
                .ForMember(dest => dest.FoundedYear, opt => opt.MapFrom(src => new DateTime(src.FoundedYear, 1, 1)))
                .ForMember(dest => dest.Teams, opt => opt.Ignore());

            CreateMap<CreateClubCommand, Club>();
            CreateMap<UpdateClubCommand, Club>();
        }
    }
}
