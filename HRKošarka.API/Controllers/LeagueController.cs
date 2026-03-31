using HRKošarka.API.Models;
using HRKošarka.Application.Features.League.Commands.ActivateLeague;
using HRKošarka.Application.Features.League.Commands.CreateLeague;
using HRKošarka.Application.Features.League.Commands.DeactivateLeague;
using HRKošarka.Application.Features.League.Commands.DeleteLeague;
using HRKošarka.Application.Features.League.Commands.UpdateLeague;
using HRKošarka.Application.Features.League.Queries.GetAllLeagues;
using HRKošarka.Application.Features.League.Queries.GetLeagueDetails;
using HRKošarka.Application.Models.Responses;
using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace HRKošarka.API.Controllers
{
    [Route("api/leagues")]
    [ApiController]
    public class LeagueController : ControllerBase
    {
        private readonly IMediator _mediator;

        public LeagueController(IMediator mediator)
        {
            _mediator = mediator;
        }

        [HttpGet(Name = "GetAllLeagues")]
        [ProducesResponseType(typeof(PaginatedResponse<LeagueDTO>), StatusCodes.Status200OK)]
        [ProducesDefaultResponseType]
        public async Task<ActionResult<PaginatedResponse<LeagueDTO>>> Get([FromQuery] GetLeaguesQuery query)
        {
            var response = await _mediator.Send(query);
            return Ok(response);
        }

        [HttpGet("{id}", Name = "GetLeagueById")]
        [ProducesResponseType(typeof(QueryResponse<LeagueDetailsDTO>), StatusCodes.Status200OK)]
        [ProducesResponseType(typeof(CustomProblemDetails), StatusCodes.Status404NotFound)]
        [ProducesDefaultResponseType]
        public async Task<ActionResult<QueryResponse<LeagueDetailsDTO>>> Get(Guid id)
        {
            var response = await _mediator.Send(new GetLeagueDetailsQuery(id));
            return Ok(response);
        }

        [HttpPost(Name = "CreateLeague")]
        [Authorize(Roles = "Administrator")]
        [ProducesResponseType(typeof(CommandResponse<Guid>), StatusCodes.Status201Created)]
        [ProducesResponseType(typeof(CustomProblemDetails), StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status401Unauthorized)]
        [ProducesResponseType(StatusCodes.Status403Forbidden)]
        [ProducesDefaultResponseType]
        public async Task<ActionResult<CommandResponse<Guid>>> Post(CreateLeagueCommand command)
        {
            var response = await _mediator.Send(command);
            return CreatedAtAction(nameof(Get), new { id = response.Data }, response);
        }

        [HttpPut("{id}", Name = "UpdateLeague")]
        [Authorize(Roles = "Administrator")]
        [ProducesResponseType(StatusCodes.Status204NoContent)]
        [ProducesResponseType(typeof(CustomProblemDetails), StatusCodes.Status400BadRequest)]
        [ProducesResponseType(typeof(CustomProblemDetails), StatusCodes.Status404NotFound)]
        [ProducesResponseType(StatusCodes.Status401Unauthorized)]
        [ProducesResponseType(StatusCodes.Status403Forbidden)]
        [ProducesDefaultResponseType]
        public async Task<ActionResult> Put(Guid id, UpdateLeagueCommand command)
        {
            command.Id = id;
            await _mediator.Send(command);
            return NoContent();
        }

        [HttpPatch("{id}/deactivate", Name = "DeactivateLeague")]
        [Authorize(Roles = "Administrator")]
        [ProducesResponseType(StatusCodes.Status204NoContent)]
        [ProducesResponseType(typeof(CustomProblemDetails), StatusCodes.Status400BadRequest)]
        [ProducesResponseType(typeof(CustomProblemDetails), StatusCodes.Status404NotFound)]
        [ProducesResponseType(StatusCodes.Status401Unauthorized)]
        [ProducesResponseType(StatusCodes.Status403Forbidden)]
        [ProducesDefaultResponseType]
        public async Task<ActionResult> Deactivate(Guid id)
        {
            await _mediator.Send(new DeactivateLeagueCommand(id));
            return NoContent();
        }

        [HttpPatch("{id}/activate", Name = "ActivateLeague")]
        [Authorize(Roles = "Administrator")]
        [ProducesResponseType(StatusCodes.Status204NoContent)]
        [ProducesResponseType(typeof(CustomProblemDetails), StatusCodes.Status400BadRequest)]
        [ProducesResponseType(typeof(CustomProblemDetails), StatusCodes.Status404NotFound)]
        [ProducesResponseType(StatusCodes.Status401Unauthorized)]
        [ProducesResponseType(StatusCodes.Status403Forbidden)]
        [ProducesDefaultResponseType]
        public async Task<ActionResult> Activate(Guid id)
        {
            await _mediator.Send(new ActivateLeagueCommand(id));
            return NoContent();
        }

        [HttpDelete("{id}", Name = "DeleteLeague")]
        [Authorize(Roles = "Administrator")]
        [ProducesResponseType(StatusCodes.Status204NoContent)]
        [ProducesResponseType(typeof(CustomProblemDetails), StatusCodes.Status404NotFound)]
        [ProducesResponseType(StatusCodes.Status401Unauthorized)]
        [ProducesResponseType(StatusCodes.Status403Forbidden)]
        [ProducesDefaultResponseType]
        public async Task<ActionResult> Delete(Guid id)
        {
            await _mediator.Send(new DeleteLeagueCommand(id));
            return NoContent();
        }
    }
}
