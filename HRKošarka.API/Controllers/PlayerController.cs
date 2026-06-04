using HRKošarka.API.Models;
using HRKošarka.Application.Features.Player.Commands.ActivatePlayer;
using HRKošarka.Application.Features.Player.Commands.CreatePlayer;
using HRKošarka.Application.Features.Player.Commands.DeactivatePlayer;
using HRKošarka.Application.Features.Player.Commands.DeletePlayer;
using HRKošarka.Application.Features.Player.Commands.UpdatePlayer;
using HRKošarka.Application.Features.Player.Queries.GetAllPlayers;
using HRKošarka.Application.Features.Player.Queries.GetAvailablePlayers;
using HRKošarka.Application.Features.Player.Queries.GetPlayerAssignments;
using HRKošarka.Application.Features.Team.Queries.GetAvailableTeamsForPlayer;
using HRKošarka.Application.Features.Player.Queries.GetPlayerDetails;
using HRKošarka.Application.Features.Player.Queries.GetPlayerCareer;
using HRKošarka.Application.Features.Player.Queries.GetPlayerSeasonStats;
using HRKošarka.Application.Models.Responses;
using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace HRKošarka.API.Controllers
{
    [Route("api/players")]
    [ApiController]
    public class PlayerController : BaseController
    {
        private readonly IMediator _mediator;

        public PlayerController(IMediator mediator)
        {
            _mediator = mediator;
        }

        [HttpGet(Name = "GetAllPlayers")]
        [ProducesResponseType(typeof(PaginatedResponse<PlayerDTO>), StatusCodes.Status200OK)]
        [ProducesResponseType(typeof(PaginatedResponse<PlayerDTO>), StatusCodes.Status500InternalServerError)]
        [ProducesDefaultResponseType]
        public async Task<ActionResult<PaginatedResponse<PlayerDTO>>> Get([FromQuery] GetPlayersQuery query)
        {
            var response = await _mediator.Send(query);
            return Ok(response);
        }

        [HttpGet("available", Name = "GetAvailablePlayers")]
        [ProducesResponseType(typeof(QueryResponse<List<AvailablePlayerDTO>>), StatusCodes.Status200OK)]
        [ProducesDefaultResponseType]
        public async Task<ActionResult<QueryResponse<List<AvailablePlayerDTO>>>> GetAvailable([FromQuery] Guid teamId, [FromQuery] string? searchTerm)
        {
            var response = await _mediator.Send(new GetAvailablePlayersQuery { TeamId = teamId, SearchTerm = searchTerm });
            return Ok(response);
        }

        [HttpGet("{id}/assignments", Name = "GetPlayerAssignments")]
        [ProducesResponseType(typeof(QueryResponse<List<PlayerAssignmentDTO>>), StatusCodes.Status200OK)]
        [ProducesDefaultResponseType]
        public async Task<ActionResult<QueryResponse<List<PlayerAssignmentDTO>>>> GetAssignments(Guid id)
        {
            var response = await _mediator.Send(new GetPlayerAssignmentsQuery(id));
            return Ok(response);
        }

        [HttpGet("{id}/available-teams", Name = "GetAvailableTeamsForPlayer")]
        [ProducesResponseType(typeof(QueryResponse<List<AvailableTeamDTO>>), StatusCodes.Status200OK)]
        [ProducesDefaultResponseType]
        public async Task<ActionResult<QueryResponse<List<AvailableTeamDTO>>>> GetAvailableTeams(Guid id, [FromQuery] string? searchTerm)
        {
            var response = await _mediator.Send(new GetAvailableTeamsForPlayerQuery { PlayerId = id, SearchTerm = searchTerm });
            return Ok(response);
        }

        [HttpGet("{id}/career", Name = "GetPlayerCareer")]
        [ProducesResponseType(typeof(QueryResponse<List<PlayerCareerItemDTO>>), StatusCodes.Status200OK)]
        [ProducesDefaultResponseType]
        public async Task<ActionResult<QueryResponse<List<PlayerCareerItemDTO>>>> GetCareer(Guid id)
        {
            var response = await _mediator.Send(new GetPlayerCareerQuery(id));
            return Ok(response);
        }

        [HttpGet("{id}/season-stats", Name = "GetPlayerSeasonStats")]
        [ProducesResponseType(typeof(QueryResponse<List<PlayerSeasonGroupDTO>>), StatusCodes.Status200OK)]
        [ProducesDefaultResponseType]
        public async Task<ActionResult<QueryResponse<List<PlayerSeasonGroupDTO>>>> GetSeasonStats(Guid id)
        {
            var response = await _mediator.Send(new GetPlayerSeasonStatsQuery(id));
            return Ok(response);
        }

        [HttpGet("{id}", Name = "GetPlayerById")]
        [ProducesResponseType(typeof(QueryResponse<PlayerDetailsDTO>), StatusCodes.Status200OK)]
        [ProducesResponseType(typeof(QueryResponse<PlayerDetailsDTO>), StatusCodes.Status404NotFound)]
        [ProducesDefaultResponseType]
        public async Task<ActionResult<QueryResponse<PlayerDetailsDTO>>> Get(Guid id)
        {
            var response = await _mediator.Send(new GetPlayerDetailsQuery(id));
            return Ok(response);
        }

        [HttpPost(Name = "CreatePlayer")]
        [Authorize(Roles = "Administrator")]
        [ProducesResponseType(typeof(CommandResponse<Guid>), StatusCodes.Status201Created)]
        [ProducesResponseType(typeof(CustomProblemDetails), StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status401Unauthorized)]
        [ProducesResponseType(StatusCodes.Status403Forbidden)]
        [ProducesDefaultResponseType]
        public async Task<ActionResult<CommandResponse<Guid>>> Post(CreatePlayerCommand player)
        {
            var response = await _mediator.Send(player);
            return CreatedAtAction(nameof(Get), new { id = response.Data }, response);
        }

        [HttpPut("{id}", Name = "UpdatePlayer")]
        [Authorize(Roles = "Administrator")]
        [ProducesResponseType(StatusCodes.Status204NoContent)]
        [ProducesResponseType(typeof(CustomProblemDetails), StatusCodes.Status400BadRequest)]
        [ProducesResponseType(typeof(CustomProblemDetails), StatusCodes.Status404NotFound)]
        [ProducesResponseType(StatusCodes.Status401Unauthorized)]
        [ProducesResponseType(StatusCodes.Status403Forbidden)]
        [ProducesDefaultResponseType]
        public async Task<ActionResult> Put(Guid id, UpdatePlayerCommand player)
        {
            player.Id = id;
            await _mediator.Send(player);
            return NoContent();
        }

        [HttpPatch("{id}/deactivate", Name = "DeactivatePlayer")]
        [Authorize(Roles = "Administrator")]
        [ProducesResponseType(StatusCodes.Status204NoContent)]
        [ProducesResponseType(typeof(CustomProblemDetails), StatusCodes.Status400BadRequest)]
        [ProducesResponseType(typeof(CustomProblemDetails), StatusCodes.Status404NotFound)]
        [ProducesResponseType(StatusCodes.Status401Unauthorized)]
        [ProducesResponseType(StatusCodes.Status403Forbidden)]
        [ProducesDefaultResponseType]
        public async Task<ActionResult> Deactivate(Guid id)
        {
            await _mediator.Send(new DeactivatePlayerCommand(id));
            return NoContent();
        }

        [HttpPatch("{id}/activate", Name = "ActivatePlayer")]
        [Authorize(Roles = "Administrator")]
        [ProducesResponseType(StatusCodes.Status204NoContent)]
        [ProducesResponseType(typeof(CustomProblemDetails), StatusCodes.Status400BadRequest)]
        [ProducesResponseType(typeof(CustomProblemDetails), StatusCodes.Status404NotFound)]
        [ProducesResponseType(StatusCodes.Status401Unauthorized)]
        [ProducesResponseType(StatusCodes.Status403Forbidden)]
        [ProducesDefaultResponseType]
        public async Task<ActionResult> Activate(Guid id)
        {
            await _mediator.Send(new ActivatePlayerCommand(id));
            return NoContent();
        }

        [HttpDelete("{id}", Name = "DeletePlayer")]
        [Authorize(Roles = "Administrator")]
        [ProducesResponseType(StatusCodes.Status204NoContent)]
        [ProducesResponseType(typeof(CustomProblemDetails), StatusCodes.Status400BadRequest)]
        [ProducesResponseType(typeof(CustomProblemDetails), StatusCodes.Status404NotFound)]
        [ProducesResponseType(StatusCodes.Status401Unauthorized)]
        [ProducesResponseType(StatusCodes.Status403Forbidden)]
        [ProducesDefaultResponseType]
        public async Task<ActionResult> Delete(Guid id)
        {
            await _mediator.Send(new DeletePlayerCommand(id));
            return NoContent();
        }
    }
}
