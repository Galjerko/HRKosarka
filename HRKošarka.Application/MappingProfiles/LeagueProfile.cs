using AutoMapper;
using HRKošarka.Application.Features.League.Commands.CreateLeague;
using HRKošarka.Application.Features.League.Commands.UpdateLeague;
using HRKošarka.Application.Features.League.Queries.GetAllLeagues;
using HRKošarka.Application.Features.League.Queries.GetLeagueDetails;

namespace HRKošarka.Application.MappingProfiles
{
    public class LeagueProfile : Profile
    {
        public LeagueProfile()
        {
            CreateMap<Domain.League, LeagueDTO>();
            CreateMap<Domain.League, LeagueDetailsDTO>();
            CreateMap<CreateLeagueCommand, Domain.League>();
            CreateMap<UpdateLeagueCommand, Domain.League>();
        }
    }
}
