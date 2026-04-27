using AutoMapper;
using HRKošarka.Application.Features.Player.Queries.GetPlayerAssignments;
using HRKošarka.Application.Features.Team.Commands.CreateTeam;
using HRKošarka.Application.Features.Team.Commands.UpdateTeam;
using HRKošarka.Application.Features.Team.Queries.GetAllTeams;
using HRKošarka.Application.Features.Team.Queries.GetTeamDetails;
using HRKošarka.Application.Features.Team.Queries.GetTeamRoster;
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

            CreateMap<PlayerTeamHistory, TeamRosterPlayerDTO>()
                .ForMember(dest => dest.PlayerFirstName, opt => opt.MapFrom(src => src.Player.FirstName))
                .ForMember(dest => dest.PlayerLastName, opt => opt.MapFrom(src => src.Player.LastName))
                .ForMember(dest => dest.RegistrationNumber, opt => opt.MapFrom(src => src.Player.RegistrationNumber))
                .ForMember(dest => dest.DateOfBirth, opt => opt.MapFrom(src => src.Player.DateOfBirth))
                .ForMember(dest => dest.Position, opt => opt.MapFrom(src => src.Player.Position))
                .ForMember(dest => dest.SeasonName, opt => opt.MapFrom(src => src.Season.Name));

            CreateMap<PlayerTeamHistory, PlayerAssignmentDTO>()
                .ForMember(dest => dest.TeamName, opt => opt.MapFrom(src => src.Team.Name))
                .ForMember(dest => dest.ClubId, opt => opt.MapFrom(src => src.Team.ClubId))
                .ForMember(dest => dest.ClubName, opt => opt.MapFrom(src => src.Team.Club.Name))
                .ForMember(dest => dest.AgeCategoryName, opt => opt.MapFrom(src => src.Team.AgeCategory.Name))
                .ForMember(dest => dest.Gender, opt => opt.MapFrom(src => src.Team.Gender))
                .ForMember(dest => dest.SeasonName, opt => opt.MapFrom(src => src.Season.Name));
        }
    }
}
