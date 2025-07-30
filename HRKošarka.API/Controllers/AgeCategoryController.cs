using HRKošarka.Application.Features.AgeCategory.Queries.GetAllAgeCategories;
using HRKošarka.Application.Models.Responses;
using MediatR;
using Microsoft.AspNetCore.Mvc;

namespace HRKošarka.API.Controllers
{
    [Route("api/age-categories")]
    [ApiController]
    public class AgeCategoryController : ControllerBase
    {
        private readonly IMediator _mediator;

        public AgeCategoryController(IMediator mediator)
        {
            _mediator = mediator;
        }

        [HttpGet(Name = "GetAllAgeCategories")]
        [ProducesResponseType(typeof(QueryResponse<List<AgeCategoryDTO>>), StatusCodes.Status200OK)]
        [ProducesResponseType(typeof(QueryResponse<List<AgeCategoryDTO>>), StatusCodes.Status500InternalServerError)]
        [ProducesDefaultResponseType]
        public async Task<ActionResult<QueryResponse<List<AgeCategoryDTO>>>> Get()
        {
            var response = await _mediator.Send(new GetAllAgeCategoriesQuery());
            return Ok(response);
        }
    }
}
