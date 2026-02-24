using AutoMapper;
using HRKošarka.Application.Features.Season.Commands.CreateSeason;
using HRKošarka.Application.Features.Season.Commands.UpdateSeason;
using HRKošarka.Application.Features.Season.Queries.GetAllSeasons;
using HRKošarka.Application.Features.Season.Queries.GetSeasonDetails;

namespace HRKošarka.Application.MappingProfiles
{
    public class SeasonProfile : Profile
    {
        public SeasonProfile()
        {
            CreateMap<Domain.Season, SeasonDTO>();
            CreateMap<Domain.Season, SeasonDetailsDTO>();
            CreateMap<CreateSeasonCommand, Domain.Season>();
            CreateMap<UpdateSeasonCommand, Domain.Season>();
        }
    }
}