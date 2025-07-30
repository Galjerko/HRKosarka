using AutoMapper;
using HRKošarka.Application.Features.AgeCategory.Queries.GetAllAgeCategories;
using HRKošarka.Domain;

namespace HRKošarka.Application.MappingProfiles
{
    public class AgeCategoryProfile : Profile
    {
        public AgeCategoryProfile()
        {
            CreateMap<AgeCategory, AgeCategoryDTO>();
        }
    }
}
