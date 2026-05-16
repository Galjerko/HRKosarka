using HRKošarka.API.Models;
using HRKošarka.Application.Features.League.Commands.ActivateLeague;
using HRKošarka.Application.Features.League.Commands.AddLeagueBreak;
using HRKošarka.Application.Features.League.Commands.CreateLeague;
using HRKošarka.Application.Features.League.Commands.DeactivateLeague;
using HRKošarka.Application.Features.League.Commands.DeleteLeague;
using HRKošarka.Application.Features.League.Commands.GenerateLeagueSchedule;
using HRKošarka.Application.Features.League.Commands.RegisterTeamInLeague;
using HRKošarka.Application.Features.League.Commands.RemoveLeagueBreak;
using HRKošarka.Application.Features.League.Commands.RemoveTeamFromLeague;
using HRKošarka.Application.Features.League.Commands.UpdateLeague;
using HRKošarka.Application.Features.League.Queries.GetAllLeagues;
using HRKošarka.Application.Features.League.Queries.GetAvailableTeamsForLeague;
using HRKošarka.Application.Features.League.Queries.GetFeaturedLeagueMatches;
using HRKošarka.Application.Features.League.Queries.GetLeagueBreaks;
using HRKošarka.Application.Features.League.Queries.GetLeagueDetails;
using HRKošarka.Application.Features.League.Queries.GetLeagueSchedule;
using HRKošarka.Application.Features.League.Queries.GetLeagueTeams;
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

        [HttpGet("featured-matches", Name = "GetFeaturedLeagueMatches")]
        [ProducesResponseType(typeof(QueryResponse<List<FeaturedLeagueRoundDTO>>), StatusCodes.Status200OK)]
        [ProducesDefaultResponseType]
        public async Task<ActionResult<QueryResponse<List<FeaturedLeagueRoundDTO>>>> GetFeaturedMatches()
        {
            var response = await _mediator.Send(new GetFeaturedLeagueMatchesQuery());
            return Ok(response);
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

        [HttpGet("{id}/teams", Name = "GetLeagueTeams")]
        [ProducesResponseType(typeof(QueryResponse<List<LeagueTeamDTO>>), StatusCodes.Status200OK)]
        [ProducesDefaultResponseType]
        public async Task<ActionResult<QueryResponse<List<LeagueTeamDTO>>>> GetTeams(Guid id)
        {
            var response = await _mediator.Send(new GetLeagueTeamsQuery(id));
            return Ok(response);
        }

        [HttpGet("{id}/available-teams", Name = "GetAvailableTeamsForLeague")]
        [ProducesResponseType(typeof(QueryResponse<List<AvailableTeamForLeagueDTO>>), StatusCodes.Status200OK)]
        [ProducesDefaultResponseType]
        public async Task<ActionResult<QueryResponse<List<AvailableTeamForLeagueDTO>>>> GetAvailableTeams(Guid id, [FromQuery] string? searchTerm)
        {
            var response = await _mediator.Send(new GetAvailableTeamsForLeagueQuery { LeagueId = id, SearchTerm = searchTerm });
            return Ok(response);
        }

        [HttpPost("{id}/teams", Name = "RegisterTeamInLeague")]
        [Authorize(Roles = "Administrator")]
        [ProducesResponseType(typeof(CommandResponse<Guid>), StatusCodes.Status201Created)]
        [ProducesResponseType(typeof(CustomProblemDetails), StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status401Unauthorized)]
        [ProducesResponseType(StatusCodes.Status403Forbidden)]
        [ProducesDefaultResponseType]
        public async Task<ActionResult<CommandResponse<Guid>>> RegisterTeam(Guid id, RegisterTeamInLeagueCommand command)
        {
            command.LeagueId = id;
            var response = await _mediator.Send(command);
            return CreatedAtAction(nameof(GetTeams), new { id }, response);
        }

        [HttpDelete("{id}/teams/{teamId}", Name = "RemoveTeamFromLeague")]
        [Authorize(Roles = "Administrator")]
        [ProducesResponseType(StatusCodes.Status204NoContent)]
        [ProducesResponseType(typeof(CustomProblemDetails), StatusCodes.Status404NotFound)]
        [ProducesResponseType(StatusCodes.Status401Unauthorized)]
        [ProducesResponseType(StatusCodes.Status403Forbidden)]
        [ProducesDefaultResponseType]
        public async Task<ActionResult> RemoveTeam(Guid id, Guid teamId)
        {
            await _mediator.Send(new RemoveTeamFromLeagueCommand { LeagueId = id, TeamId = teamId });
            return NoContent();
        }

        [HttpGet("{id}/breaks", Name = "GetLeagueBreaks")]
        [ProducesResponseType(typeof(QueryResponse<List<LeagueBreakDTO>>), StatusCodes.Status200OK)]
        [ProducesDefaultResponseType]
        public async Task<ActionResult<QueryResponse<List<LeagueBreakDTO>>>> GetBreaks(Guid id)
        {
            var response = await _mediator.Send(new GetLeagueBreaksQuery(id));
            return Ok(response);
        }

        [HttpPost("{id}/breaks", Name = "AddLeagueBreak")]
        [Authorize(Roles = "Administrator")]
        [ProducesResponseType(typeof(CommandResponse<Guid>), StatusCodes.Status201Created)]
        [ProducesResponseType(typeof(CustomProblemDetails), StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status401Unauthorized)]
        [ProducesResponseType(StatusCodes.Status403Forbidden)]
        [ProducesDefaultResponseType]
        public async Task<ActionResult<CommandResponse<Guid>>> AddBreak(Guid id, AddLeagueBreakCommand command)
        {
            command.LeagueId = id;
            var response = await _mediator.Send(command);
            return CreatedAtAction(nameof(GetBreaks), new { id }, response);
        }

        [HttpDelete("{id}/breaks/{breakId}", Name = "RemoveLeagueBreak")]
        [Authorize(Roles = "Administrator")]
        [ProducesResponseType(StatusCodes.Status204NoContent)]
        [ProducesResponseType(typeof(CustomProblemDetails), StatusCodes.Status404NotFound)]
        [ProducesResponseType(StatusCodes.Status401Unauthorized)]
        [ProducesResponseType(StatusCodes.Status403Forbidden)]
        [ProducesDefaultResponseType]
        public async Task<ActionResult> RemoveBreak(Guid id, Guid breakId)
        {
            await _mediator.Send(new RemoveLeagueBreakCommand(breakId));
            return NoContent();
        }

        [HttpGet("{id}/schedule", Name = "GetLeagueSchedule")]
        [ProducesResponseType(typeof(QueryResponse<List<LeagueRoundDTO>>), StatusCodes.Status200OK)]
        [ProducesDefaultResponseType]
        public async Task<ActionResult<QueryResponse<List<LeagueRoundDTO>>>> GetSchedule(Guid id)
        {
            var response = await _mediator.Send(new GetLeagueScheduleQuery(id));
            return Ok(response);
        }

        [HttpPost("{id}/generate-schedule", Name = "GenerateLeagueSchedule")]
        [Authorize(Roles = "Administrator")]
        [ProducesResponseType(typeof(CommandResponse<int>), StatusCodes.Status200OK)]
        [ProducesResponseType(typeof(CustomProblemDetails), StatusCodes.Status400BadRequest)]
        [ProducesResponseType(typeof(CustomProblemDetails), StatusCodes.Status404NotFound)]
        [ProducesResponseType(StatusCodes.Status401Unauthorized)]
        [ProducesResponseType(StatusCodes.Status403Forbidden)]
        [ProducesDefaultResponseType]
        public async Task<ActionResult<CommandResponse<int>>> GenerateSchedule(Guid id)
        {
            var response = await _mediator.Send(new GenerateLeagueScheduleCommand(id));
            return Ok(response);
        }
    }
}
