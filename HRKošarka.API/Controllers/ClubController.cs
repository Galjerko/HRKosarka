using HRKošarka.API.Models;
using HRKošarka.Application.Features.Club.Commands.ActivateClub;
using HRKošarka.Application.Features.Club.Commands.CreateClub;
using HRKošarka.Application.Features.Club.Commands.DeactivateClub;
using HRKošarka.Application.Features.Club.Commands.DeleteClub;
using HRKošarka.Application.Features.Club.Commands.UpdateClub;
using HRKošarka.Application.Features.Club.Queries.GetAllClubs;
using HRKošarka.Application.Features.Club.Queries.GetClubDetails;
using HRKošarka.Application.Features.Club.Queries.GetClubsWithoutManager;
using HRKošarka.Application.Features.Club.Queries.GetClubsWIthoutManager;
using HRKošarka.Application.Models.Responses;
using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace HRKošarka.API.Controllers
{
    [Route("api/clubs")]
    [ApiController]
    public class ClubController : ControllerBase
    {
        private readonly IMediator _mediator;

        public ClubController(IMediator mediator)
        {
            _mediator = mediator;
        }

        [HttpGet(Name = "GetAllClubs")]
        [ProducesResponseType(typeof(PaginatedResponse<ClubDTO>), StatusCodes.Status200OK)]
        [ProducesResponseType(typeof(PaginatedResponse<ClubDTO>), StatusCodes.Status500InternalServerError)]
        [ProducesDefaultResponseType]
        public async Task<ActionResult<PaginatedResponse<ClubDTO>>> Get([FromQuery] GetClubsQuery query)
        {
            var response = await _mediator.Send(query);
            return Ok(response);

        }

        [HttpGet("{id}", Name = "GetClubById")]
        [ProducesResponseType(typeof(QueryResponse<ClubDetailsDTO>), StatusCodes.Status200OK)]
        [ProducesResponseType(typeof(QueryResponse<ClubDetailsDTO>), StatusCodes.Status404NotFound)]
        [ProducesDefaultResponseType]
        public async Task<ActionResult<QueryResponse<ClubDetailsDTO>>> Get(Guid id)
        {
            var response = await _mediator.Send(new GetClubDetailsQuery(id));
            return Ok(response);

        }

        [HttpPost(Name = "CreateClub")]
        [Authorize(Roles = "Administrator")]
        [ProducesResponseType(typeof(CommandResponse<Guid>), StatusCodes.Status201Created)]
        [ProducesResponseType(typeof(CustomProblemDetails), StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status401Unauthorized)]
        [ProducesResponseType(StatusCodes.Status403Forbidden)]
        [ProducesDefaultResponseType]
        public async Task<ActionResult<CommandResponse<Guid>>> Post(CreateClubCommand club)
        {
            var response = await _mediator.Send(club);
            return CreatedAtAction(nameof(Get), new { id = response.Data }, response);
        }


        [HttpPut("{id}", Name = "UpdateClub")]
        [Authorize(Roles = "Administrator, ClubManager")]
        [ProducesResponseType(StatusCodes.Status204NoContent)]
        [ProducesResponseType(typeof(CustomProblemDetails), StatusCodes.Status400BadRequest)]
        [ProducesResponseType(typeof(CustomProblemDetails), StatusCodes.Status404NotFound)]
        [ProducesResponseType(StatusCodes.Status401Unauthorized)]
        [ProducesResponseType(StatusCodes.Status403Forbidden)]
        [ProducesDefaultResponseType]
        public async Task<ActionResult> Put(Guid id, UpdateClubCommand club)
        {
            club.Id = id;
            await _mediator.Send(club);
            return NoContent();
        }

        [HttpPatch("{id}/deactivate", Name = "DeactivateClub")]
        [Authorize(Roles = "Administrator,ClubManager")]
        [ProducesResponseType(StatusCodes.Status204NoContent)]
        [ProducesResponseType(typeof(CustomProblemDetails), StatusCodes.Status400BadRequest)]
        [ProducesResponseType(typeof(CustomProblemDetails), StatusCodes.Status404NotFound)]
        [ProducesResponseType(StatusCodes.Status401Unauthorized)]
        [ProducesResponseType(StatusCodes.Status403Forbidden)]
        [ProducesDefaultResponseType]
        public async Task<ActionResult> Deactivate(Guid id)
        {
            await _mediator.Send(new DeactivateClubCommand(id));
            return NoContent();
        }

        [HttpPatch("{id}/activate", Name = "ActivateClub")]
        [Authorize(Roles = "Administrator,ClubManager")]
        [ProducesResponseType(StatusCodes.Status204NoContent)]
        [ProducesResponseType(typeof(CustomProblemDetails), StatusCodes.Status400BadRequest)]
        [ProducesResponseType(typeof(CustomProblemDetails), StatusCodes.Status404NotFound)]
        [ProducesResponseType(StatusCodes.Status401Unauthorized)]
        [ProducesResponseType(StatusCodes.Status403Forbidden)]
        [ProducesDefaultResponseType]
        public async Task<ActionResult> Activate(Guid id)
        {
            await _mediator.Send(new ActivateClubCommand(id));
            return NoContent();
        }

        [HttpDelete("{id}", Name = "DeleteClub")]
        [Authorize(Roles = "Administrator")]
        [ProducesResponseType(StatusCodes.Status204NoContent)]
        [ProducesResponseType(typeof(CustomProblemDetails), StatusCodes.Status400BadRequest)]
        [ProducesResponseType(typeof(CustomProblemDetails), StatusCodes.Status404NotFound)]
        [ProducesResponseType(StatusCodes.Status401Unauthorized)]
        [ProducesResponseType(StatusCodes.Status403Forbidden)]
        [ProducesDefaultResponseType]
        public async Task<ActionResult> Delete(Guid id)
        {
            await _mediator.Send(new DeleteClubCommand(id));
            return NoContent();
        }

        [HttpGet("without-manager", Name = "GetClubsWithoutManager")]
        [Authorize(Roles = "Administrator")]
        [ProducesResponseType(typeof(QueryResponse<List<ClubWithoutManagerDTO>>), StatusCodes.Status200OK)]
        [ProducesResponseType(typeof(QueryResponse<List<ClubWithoutManagerDTO>>), StatusCodes.Status500InternalServerError)]
        [ProducesResponseType(StatusCodes.Status401Unauthorized)]
        [ProducesResponseType(StatusCodes.Status403Forbidden)]
        [ProducesDefaultResponseType]
        public async Task<ActionResult<QueryResponse<List<ClubWithoutManagerDTO>>>> GetClubsWithoutManager(
                [FromQuery] string? searchTerm)
        {
            var query = new GetClubsWithoutManagerQuery
            {
                SearchTerm = searchTerm
            };

            var response = await _mediator.Send(query);
            return Ok(response);
        }
    }
}
