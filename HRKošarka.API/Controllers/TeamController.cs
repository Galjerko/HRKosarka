using HRKošarka.API.Models;
using HRKošarka.Application.Features.Team.Commands.ActivateTeam;
using HRKošarka.Application.Features.Team.Commands.AssignPlayerToTeam;
using HRKošarka.Application.Features.Team.Commands.AssignTeamRepresentative;
using HRKošarka.Application.Features.Team.Commands.CreateTeam;
using HRKošarka.Application.Features.Team.Commands.DeactivateTeam;
using HRKošarka.Application.Features.Team.Commands.DeleteTeam;
using HRKošarka.Application.Features.Team.Commands.RemovePlayerFromTeam;
using HRKošarka.Application.Features.Team.Commands.RevokeTeamRepresentative;
using HRKošarka.Application.Features.Team.Commands.UpdatePlayerAssignmentInTeam;
using HRKošarka.Application.Features.Team.Commands.UpdateTeam;
using HRKošarka.Application.Features.Team.Queries.GetAllTeams;
using HRKošarka.Application.Features.Team.Queries.GetMyRepresentativeships;
using HRKošarka.Application.Features.Team.Queries.GetTeamDetails;
using HRKošarka.Application.Features.Team.Queries.GetTeamLeagues;
using HRKošarka.Application.Features.Team.Queries.GetTeamMatchHistory;
using HRKošarka.Application.Features.Team.Queries.GetTeamRepresentatives;
using HRKošarka.Application.Features.Team.Queries.GetTeamRoster;
using HRKošarka.Application.Models.Responses;
using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;

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

        [HttpGet("my-representativeships", Name = "GetMyRepresentativeships")]
        [Authorize]
        [ProducesResponseType(typeof(QueryResponse<List<TeamRepMembershipDTO>>), StatusCodes.Status200OK)]
        [ProducesDefaultResponseType]
        public async Task<ActionResult<QueryResponse<List<TeamRepMembershipDTO>>>> GetMyRepresentativeships()
        {
            var userId = User.FindFirstValue("uid") ?? string.Empty;
            var response = await _mediator.Send(new GetMyRepresentativeshipsQuery(userId));
            return Ok(response);
        }

        [HttpGet("{id}/representatives", Name = "GetTeamRepresentatives")]
        [Authorize]
        [ProducesResponseType(typeof(QueryResponse<List<TeamRepresentativeDTO>>), StatusCodes.Status200OK)]
        [ProducesDefaultResponseType]
        public async Task<ActionResult<QueryResponse<List<TeamRepresentativeDTO>>>> GetRepresentatives(Guid id)
        {
            var response = await _mediator.Send(new GetTeamRepresentativesQuery(id));
            return Ok(response);
        }

        [HttpPost("{id}/representatives", Name = "AssignTeamRepresentative")]
        [Authorize(Roles = "Administrator")]
        [ProducesResponseType(typeof(CommandResponse<Guid>), StatusCodes.Status201Created)]
        [ProducesResponseType(typeof(CustomProblemDetails), StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status401Unauthorized)]
        [ProducesResponseType(StatusCodes.Status403Forbidden)]
        [ProducesDefaultResponseType]
        public async Task<ActionResult<CommandResponse<Guid>>> AssignRepresentative(Guid id, AssignTeamRepresentativeCommand command)
        {
            command.TeamId = id;
            var response = await _mediator.Send(command);
            return CreatedAtAction(nameof(GetRepresentatives), new { id }, response);
        }

        [HttpDelete("{id}/representatives/{repId}", Name = "RevokeTeamRepresentative")]
        [Authorize(Roles = "Administrator")]
        [ProducesResponseType(StatusCodes.Status204NoContent)]
        [ProducesResponseType(typeof(CustomProblemDetails), StatusCodes.Status400BadRequest)]
        [ProducesResponseType(typeof(CustomProblemDetails), StatusCodes.Status404NotFound)]
        [ProducesResponseType(StatusCodes.Status401Unauthorized)]
        [ProducesResponseType(StatusCodes.Status403Forbidden)]
        [ProducesDefaultResponseType]
        public async Task<ActionResult> RevokeRepresentative(Guid id, Guid repId)
        {
            await _mediator.Send(new RevokeTeamRepresentativeCommand { TeamId = id, RepresentativeId = repId });
            return NoContent();
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
        [Authorize]
        [ProducesResponseType(StatusCodes.Status204NoContent)]
        [ProducesResponseType(typeof(CustomProblemDetails), StatusCodes.Status400BadRequest)]
        [ProducesResponseType(typeof(CustomProblemDetails), StatusCodes.Status404NotFound)]
        [ProducesResponseType(StatusCodes.Status401Unauthorized)]
        [ProducesResponseType(StatusCodes.Status403Forbidden)]
        [ProducesDefaultResponseType]
        public async Task<ActionResult> Put(Guid id, UpdateTeamCommand team)
        {
            team.Id = id;
            team.RequesterClubId = User.FindFirstValue("ClubId");
            team.RequesterUserId = User.IsInRole("Administrator") ? null : User.FindFirstValue("uid");
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

        [HttpGet("{id}/leagues", Name = "GetTeamLeagues")]
        [ProducesResponseType(typeof(QueryResponse<List<TeamLeagueDTO>>), StatusCodes.Status200OK)]
        [ProducesDefaultResponseType]
        public async Task<ActionResult<QueryResponse<List<TeamLeagueDTO>>>> GetLeagues(Guid id)
        {
            var response = await _mediator.Send(new GetTeamLeaguesQuery(id));
            return Ok(response);
        }

        [HttpGet("{id}/matches", Name = "GetTeamMatchHistory")]
        [ProducesResponseType(typeof(QueryResponse<List<TeamMatchHistoryItemDTO>>), StatusCodes.Status200OK)]
        [ProducesDefaultResponseType]
        public async Task<ActionResult<QueryResponse<List<TeamMatchHistoryItemDTO>>>> GetMatchHistory(Guid id)
        {
            var response = await _mediator.Send(new GetTeamMatchHistoryQuery(id));
            return Ok(response);
        }

        [HttpPost("{id}/players", Name = "AssignPlayerToTeam")]
        [Authorize]
        [ProducesResponseType(typeof(CommandResponse<Guid>), StatusCodes.Status201Created)]
        [ProducesResponseType(typeof(CustomProblemDetails), StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status401Unauthorized)]
        [ProducesResponseType(StatusCodes.Status403Forbidden)]
        [ProducesDefaultResponseType]
        public async Task<ActionResult<CommandResponse<Guid>>> AssignPlayer(Guid id, AssignPlayerToTeamCommand command)
        {
            command.TeamId = id;
            command.RequesterClubId = User.FindFirstValue("ClubId");
            command.RequesterUserId = User.IsInRole("Administrator") ? null : User.FindFirstValue("uid");
            var response = await _mediator.Send(command);
            return CreatedAtAction(nameof(Get), new { id }, response);
        }

        [HttpPut("{id}/players/{playerId}", Name = "UpdatePlayerAssignmentInTeam")]
        [Authorize]
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
            command.RequesterClubId = User.FindFirstValue("ClubId");
            command.RequesterUserId = User.IsInRole("Administrator") ? null : User.FindFirstValue("uid");
            await _mediator.Send(command);
            return NoContent();
        }

        [HttpDelete("{id}/players/{playerId}", Name = "RemovePlayerFromTeam")]
        [Authorize]
        [ProducesResponseType(StatusCodes.Status204NoContent)]
        [ProducesResponseType(typeof(CustomProblemDetails), StatusCodes.Status400BadRequest)]
        [ProducesResponseType(typeof(CustomProblemDetails), StatusCodes.Status404NotFound)]
        [ProducesResponseType(StatusCodes.Status401Unauthorized)]
        [ProducesResponseType(StatusCodes.Status403Forbidden)]
        [ProducesDefaultResponseType]
        public async Task<ActionResult> RemovePlayer(Guid id, Guid playerId)
        {
            var clubId = User.FindFirstValue("ClubId");
            var userId = User.IsInRole("Administrator") ? null : User.FindFirstValue("uid");
            await _mediator.Send(new RemovePlayerFromTeamCommand
            {
                TeamId = id,
                PlayerId = playerId,
                RequesterClubId = clubId,
                RequesterUserId = userId
            });
            return NoContent();
        }
    }
}
