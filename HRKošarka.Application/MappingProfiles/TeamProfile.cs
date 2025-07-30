using AutoMapper;
using HRKošarka.Application.Features.Team.Commands.CreateTeam;
using HRKošarka.Application.Features.Team.Commands.UpdateTeam;
using HRKošarka.Application.Features.Team.Queries.GetAllTeams;
using HRKošarka.Application.Features.Team.Queries.GetTeamDetails;
using HRKošarka.Domain;

namespace HRKošarka.Application.MappingProfiles
{
    public class TeamProfile : Profile
    {
        public TeamProfile()
        {
            CreateMap<TeamDTO, Team>().ReverseMap()
                .ForMember(dest => dest.ClubName, opt => opt.MapFrom(src => src.Club.Name))
                .ForMember(dest => dest.AgeCategoryName, opt => opt.MapFrom(src => src.AgeCategory.Name));

            CreateMap<Team, TeamDetailsDTO>()
                .ForMember(dest => dest.ClubName, opt => opt.MapFrom(src => src.Club.Name))
                .ForMember(dest => dest.AgeCategoryName, opt => opt.MapFrom(src => src.AgeCategory.Name));

            CreateMap<TeamDetailsDTO, Team>()
                .ForMember(dest => dest.Club, opt => opt.Ignore())
                .ForMember(dest => dest.AgeCategory, opt => opt.Ignore());

            CreateMap<CreateTeamCommand, Team>();
            CreateMap<UpdateTeamCommand, Team>();
        }
    }
}
