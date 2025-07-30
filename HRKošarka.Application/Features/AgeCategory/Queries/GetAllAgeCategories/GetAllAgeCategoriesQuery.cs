using HRKošarka.Application.Models.Responses;
using MediatR;

namespace HRKošarka.Application.Features.AgeCategory.Queries.GetAllAgeCategories
{
    public class GetAllAgeCategoriesQuery : IRequest<QueryResponse<List<AgeCategoryDTO>>>;
}