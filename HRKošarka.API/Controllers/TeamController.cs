using HRKošarka.API.Models;
using HRKošarka.Application.Features.Team.Commands.ActivateTeam;
using HRKošarka.Application.Features.Team.Commands.AssignPlayerToTeam;
using HRKošarka.Application.Features.Team.Commands.CreateTeam;
using HRKošarka.Application.Features.Team.Commands.DeactivateTeam;
using HRKošarka.Application.Features.Team.Commands.DeleteTeam;
using HRKošarka.Application.Features.Team.Commands.RemovePlayerFromTeam;
using HRKošarka.Application.Features.Team.Commands.UpdatePlayerAssignmentInTeam;
using HRKošarka.Application.Features.Team.Commands.UpdateTeam;
using HRKošarka.Application.Features.Team.Queries.GetAllTeams;
using HRKošarka.Application.Features.Team.Queries.GetTeamDetails;
using HRKošarka.Application.Features.Team.Queries.GetTeamRoster;
using HRKošarka.Application.Models.Responses;
using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace HRKošarka.API.Controllers
{
    [Route("api/teams")]
    [ApiController]
    public class TeamController : ControllerBase
    {
        private readonly IMediator _mediator;

        public TeamController(IMediator mediator)
        {
            _mediator = mediator;
        }

        [HttpGet(Name = "GetAllTeams")]
        [ProducesResponseType(typeof(PaginatedResponse<TeamDTO>), StatusCodes.Status200OK)]
        [ProducesResponseType(typeof(PaginatedResponse<TeamDTO>), StatusCodes.Status500InternalServerError)]
        [ProducesDefaultResponseType]
        public async Task<ActionResult<PaginatedResponse<TeamDTO>>> Get([FromQuery] GetTeamsQuery query)
        {
            var response = await _mediator.Send(query);
            return Ok(response);
        }

        [HttpGet("{id}", Name = "GetTeamById")]
        [ProducesResponseType(typeof(QueryResponse<TeamDetailsDTO>), StatusCodes.Status200OK)]
        [ProducesResponseType(typeof(QueryResponse<TeamDetailsDTO>), StatusCodes.Status404NotFound)]
        [ProducesDefaultResponseType]
        public async Task<ActionResult<QueryResponse<TeamDetailsDTO>>> Get(Guid id)
        {
            var response = await _mediator.Send(new GetTeamDetailsQuery(id));
            return Ok(response);
        }

        [HttpPost(Name = "CreateTeam")]
        [Authorize(Roles = "Administrator, ClubManager")]
        [ProducesResponseType(typeof(CommandResponse<Guid>), StatusCodes.Status201Created)]
        [ProducesResponseType(typeof(CustomProblemDetails), StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status401Unauthorized)]
        [ProducesResponseType(StatusCodes.Status403Forbidden)]
        [ProducesDefaultResponseType]
        public async Task<ActionResult<CommandResponse<Guid>>> Post(CreateTeamCommand team)
        {
            var response = await _mediator.Send(team);
            return CreatedAtAction(nameof(Get), new { id = response.Data }, response);
        }

        [HttpPut("{id}", Name = "UpdateTeam")]
        [Authorize(Roles = "Administrator, ClubManager")]
        [ProducesResponseType(StatusCodes.Status204NoContent)]
        [ProducesResponseType(typeof(CustomProblemDetails), StatusCodes.Status400BadRequest)]
        [ProducesResponseType(typeof(CustomProblemDetails), StatusCodes.Status404NotFound)]
        [ProducesResponseType(StatusCodes.Status401Unauthorized)]
        [ProducesResponseType(StatusCodes.Status403Forbidden)]
        [ProducesDefaultResponseType]
        public async Task<ActionResult> Put(Guid id, UpdateTeamCommand team)
        {
            team.Id = id;
            await _mediator.Send(team);
            return NoContent();
        }

        [HttpPatch("{id}/deactivate", Name = "DeactivateTeam")]
        [Authorize(Roles = "Administrator, ClubManager")]
        [ProducesResponseType(StatusCodes.Status204NoContent)]
        [ProducesResponseType(typeof(CustomProblemDetails), StatusCodes.Status400BadRequest)]
        [ProducesResponseType(typeof(CustomProblemDetails), StatusCodes.Status404NotFound)]
        [ProducesResponseType(StatusCodes.Status401Unauthorized)]
        [ProducesResponseType(StatusCodes.Status403Forbidden)]
        [ProducesDefaultResponseType]
        public async Task<ActionResult> Deactivate(Guid id)
        {
            await _mediator.Send(new DeactivateTeamCommand(id));
            return NoContent();
        }

        [HttpPatch("{id}/Activate", Name = "activateTeam")]
        [Authorize(Roles = "Administrator, ClubManager")]
        [ProducesResponseType(StatusCodes.Status204NoContent)]
        [ProducesResponseType(typeof(CustomProblemDetails), StatusCodes.Status400BadRequest)]
        [ProducesResponseType(typeof(CustomProblemDetails), StatusCodes.Status404NotFound)]
        [ProducesResponseType(StatusCodes.Status401Unauthorized)]
        [ProducesResponseType(StatusCodes.Status403Forbidden)]
        [ProducesDefaultResponseType]
        public async Task<ActionResult> Activate(Guid id)
        {
            await _mediator.Send(new ActivateTeamCommand(id));
            return NoContent();
        }

        [HttpDelete("{id}", Name = "DeleteTeam")]
        [Authorize(Roles = "Administrator")]
        [ProducesResponseType(StatusCodes.Status204NoContent)]
        [ProducesResponseType(typeof(CustomProblemDetails), StatusCodes.Status400BadRequest)]
        [ProducesResponseType(typeof(CustomProblemDetails), StatusCodes.Status404NotFound)]
        [ProducesResponseType(StatusCodes.Status401Unauthorized)]
        [ProducesResponseType(StatusCodes.Status403Forbidden)]
        [ProducesDefaultResponseType]
        public async Task<ActionResult> Delete(Guid id)
        {
            await _mediator.Send(new DeleteTeamCommand(id));
            return NoContent();
        }

        [HttpGet("{id}/roster", Name = "GetTeamRoster")]
        [ProducesResponseType(typeof(QueryResponse<List<TeamRosterPlayerDTO>>), StatusCodes.Status200OK)]
        [ProducesDefaultResponseType]
        public async Task<ActionResult<QueryResponse<List<TeamRosterPlayerDTO>>>> GetRoster(Guid id)
        {
            var response = await _mediator.Send(new GetTeamRosterQuery(id));
            return Ok(response);
        }

        [HttpPost("{id}/players", Name = "AssignPlayerToTeam")]
        [Authorize(Roles = "Administrator, ClubManager")]
        [ProducesResponseType(typeof(CommandResponse<Guid>), StatusCodes.Status201Created)]
        [ProducesResponseType(typeof(CustomProblemDetails), StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status401Unauthorized)]
        [ProducesResponseType(StatusCodes.Status403Forbidden)]
        [ProducesDefaultResponseType]
        public async Task<ActionResult<CommandResponse<Guid>>> AssignPlayer(Guid id, AssignPlayerToTeamCommand command)
        {
            command.TeamId = id;
            var response = await _mediator.Send(command);
            return CreatedAtAction(nameof(Get), new { id }, response);
        }

        [HttpPut("{id}/players/{playerId}", Name = "UpdatePlayerAssignmentInTeam")]
        [Authorize(Roles = "Administrator, ClubManager")]
        [ProducesResponseType(StatusCodes.Status204NoContent)]
        [ProducesResponseType(typeof(CustomProblemDetails), StatusCodes.Status400BadRequest)]
        [ProducesResponseType(typeof(CustomProblemDetails), StatusCodes.Status404NotFound)]
        [ProducesResponseType(StatusCodes.Status401Unauthorized)]
        [ProducesResponseType(StatusCodes.Status403Forbidden)]
        [ProducesDefaultResponseType]
        public async Task<ActionResult> UpdatePlayerAssignment(Guid id, Guid playerId, UpdatePlayerAssignmentInTeamCommand command)
        {
            command.TeamId = id;
            command.PlayerId = playerId;
            await _mediator.Send(command);
            return NoContent();
        }

        [HttpDelete("{id}/players/{playerId}", Name = "RemovePlayerFromTeam")]
        [Authorize(Roles = "Administrator, ClubManager")]
        [ProducesResponseType(StatusCodes.Status204NoContent)]
        [ProducesResponseType(typeof(CustomProblemDetails), StatusCodes.Status400BadRequest)]
        [ProducesResponseType(typeof(CustomProblemDetails), StatusCodes.Status404NotFound)]
        [ProducesResponseType(StatusCodes.Status401Unauthorized)]
        [ProducesResponseType(StatusCodes.Status403Forbidden)]
        [ProducesDefaultResponseType]
        public async Task<ActionResult> RemovePlayer(Guid id, Guid playerId)
        {
            await _mediator.Send(new RemovePlayerFromTeamCommand(id, playerId));
            return NoContent();
        }
    }
}
