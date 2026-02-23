using HRKošarka.Application.Features.User.Commands.AssignClubManager;
using HRKošarka.Application.Features.User.Commands.LockUser;
using HRKošarka.Application.Features.User.Commands.RemoveClubManager;
using HRKošarka.Application.Features.User.Commands.UnlockUser;
using HRKošarka.Application.Features.User.Queries.GetInactiveUsers;
using HRKošarka.Application.Features.User.Queries.GetNonAdminUsers;
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

        [HttpPost("club-managers/{userId}/lock", Name = "LockUser")]
        [Authorize(Roles = "Administrator")]
        [ProducesResponseType(StatusCodes.Status204NoContent)]
        [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status401Unauthorized)]
        [ProducesResponseType(StatusCodes.Status403Forbidden)]
        [ProducesDefaultResponseType]
        public async Task<IActionResult> LockUser(string userId)
        {
            await _mediator.Send(new LockUserCommand { UserId = userId });
            return NoContent();
        }

        [HttpPost("club-managers/{userId}/unlock", Name = "UnlockUser")]
        [Authorize(Roles = "Administrator")]
        [ProducesResponseType(StatusCodes.Status204NoContent)]
        [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status401Unauthorized)]
        [ProducesResponseType(StatusCodes.Status403Forbidden)]
        [ProducesDefaultResponseType]
        public async Task<IActionResult> UnlockUser(string userId)
        {
            await _mediator.Send(new UnlockUserCommand { UserId = userId });
            return NoContent();
        }

        [HttpGet("users", Name = "GetAllUsers")]
        [Authorize(Roles = "Administrator")]
        [ProducesResponseType(typeof(PaginatedResponse<NonAdminUserDTO>), StatusCodes.Status200OK)]
        [ProducesResponseType(typeof(PaginatedResponse<NonAdminUserDTO>), StatusCodes.Status500InternalServerError)]
        [ProducesDefaultResponseType]
        public async Task<ActionResult<PaginatedResponse<NonAdminUserDTO>>> GetUsers([FromQuery] GetNonAdminUsersQuery query)
        {
            var response = await _mediator.Send(query);
            return Ok(response);
        }

        [HttpGet("inactive-users", Name = "GetInactiveUsers")]
        [Authorize(Roles = "Administrator")]
        [ProducesResponseType(typeof(PaginatedResponse<InactiveUserDTO>), StatusCodes.Status200OK)]
        [ProducesResponseType(typeof(PaginatedResponse<InactiveUserDTO>), StatusCodes.Status500InternalServerError)]
        [ProducesDefaultResponseType]
        public async Task<ActionResult<PaginatedResponse<InactiveUserDTO>>> GetInactiveUsers([FromQuery] GetInactiveUsersQuery query)
        {
            var response = await _mediator.Send(query);
            return Ok(response);
        }
    }
}
