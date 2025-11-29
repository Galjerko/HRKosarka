using HRKošarka.Application.Features.User.Commands.AssignClubManager;
using HRKošarka.Application.Features.User.Commands.RemoveClubManager;
using HRKošarka.Application.Features.User.Queries.GetClubManagers;
using HRKošarka.Application.Models.Responses;
using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace HRKošarka.API.Controllers
{
    [Route("api/user-management")]
    [ApiController]
    public class UserManagementController : ControllerBase
    {
        private readonly IMediator _mediator;

        public UserManagementController(IMediator mediator)
        {
            _mediator = mediator;
        }

        [HttpGet("club-managers", Name = "GetAllClubManagers")]
        [ProducesResponseType(typeof(PaginatedResponse<ClubManagerDTO>), StatusCodes.Status200OK)]
        [ProducesResponseType(typeof(PaginatedResponse<ClubManagerDTO>), StatusCodes.Status500InternalServerError)]
        [ProducesDefaultResponseType]
        public async Task<ActionResult<PaginatedResponse<ClubManagerDTO>>> GetClubManagers([FromQuery] GetClubManagersQuery query)
        {
            var response = await _mediator.Send(query);
            return Ok(response);
        }

        [HttpPost("club-managers/assign", Name = "AssignClubManager")]
        [Authorize(Roles = "Administrator")]
        [ProducesResponseType(StatusCodes.Status204NoContent)]
        [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status401Unauthorized)]
        [ProducesResponseType(StatusCodes.Status403Forbidden)]
        [ProducesDefaultResponseType]
        public async Task<IActionResult> AssignClubManager([FromBody] AssignClubManagerCommand command)
        {
            await _mediator.Send(command);
            return NoContent();
        }

        [HttpPost("club-managers/{userId}/remove", Name = "RemoveClubManager")]
        [Authorize(Roles = "Administrator")]
        [ProducesResponseType(StatusCodes.Status204NoContent)]
        [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status401Unauthorized)]
        [ProducesResponseType(StatusCodes.Status403Forbidden)]
        [ProducesDefaultResponseType]
        public async Task<IActionResult> RemoveClubManager(string userId)
        {
            await _mediator.Send(new RemoveClubManagerCommand { UserId = userId });
            return NoContent();
        }
    }
}
