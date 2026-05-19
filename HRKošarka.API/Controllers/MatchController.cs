using HRKošarka.API.Models;
using HRKošarka.Application.Features.Match.Commands.ConfirmMatchResult;
using HRKošarka.Application.Features.Match.Commands.DisputeMatchResult;
using HRKošarka.Application.Features.Match.Commands.ProposeReschedule;
using HRKošarka.Application.Features.Match.Commands.RecordForfeit;
using HRKošarka.Application.Features.Match.Commands.ResetMatchResult;
using HRKošarka.Application.Features.Match.Commands.RespondToReschedule;
using HRKošarka.Application.Features.Match.Commands.SaveMatchStats;
using HRKošarka.Application.Features.Match.Queries.GetPendingActions;
using HRKošarka.Application.Features.Match.Commands.SubmitHomeStats;
using HRKošarka.Application.Features.Match.Commands.UpdateMatchVenue;
using HRKošarka.Application.Features.Match.Queries.GetMatchWithStats;
using HRKošarka.Application.Models.Responses;
using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;

namespace HRKošarka.API.Controllers
{
    [Route("api/matches")]
    [ApiController]
    public class MatchController : ControllerBase
    {
        private readonly IMediator _mediator;

        public MatchController(IMediator mediator)
        {
            _mediator = mediator;
        }

        [HttpGet("pending-actions", Name = "GetPendingActions")]
        [Authorize]
        [ProducesResponseType(typeof(QueryResponse<List<PendingActionDTO>>), StatusCodes.Status200OK)]
        [ProducesDefaultResponseType]
        public async Task<ActionResult<QueryResponse<List<PendingActionDTO>>>> GetPendingActions()
        {
            bool isAdmin = User.IsInRole("Administrator");
            Guid? clubId = null;
            if (!isAdmin && User.IsInRole("ClubManager"))
            {
                var raw = User.FindFirstValue("ClubId");
                if (!string.IsNullOrEmpty(raw) && Guid.TryParse(raw, out var parsed))
                    clubId = parsed;
            }
            var response = await _mediator.Send(new GetPendingActionsQuery { ClubId = clubId, IsAdmin = isAdmin });
            return Ok(response);
        }

        [HttpGet("{id}", Name = "GetMatchWithStats")]
        [ProducesResponseType(typeof(QueryResponse<MatchWithStatsDTO>), StatusCodes.Status200OK)]
        [ProducesResponseType(typeof(CustomProblemDetails), StatusCodes.Status404NotFound)]
        [ProducesDefaultResponseType]
        public async Task<ActionResult<QueryResponse<MatchWithStatsDTO>>> Get(Guid id)
        {
            var response = await _mediator.Send(new GetMatchWithStatsQuery(id));
            return Ok(response);
        }

        [HttpPost("{id}/stats", Name = "SaveMatchStats")]
        [Authorize(Roles = "Administrator,ClubManager")]
        [ProducesResponseType(typeof(CommandResponse<bool>), StatusCodes.Status200OK)]
        [ProducesResponseType(typeof(CustomProblemDetails), StatusCodes.Status400BadRequest)]
        [ProducesResponseType(typeof(CustomProblemDetails), StatusCodes.Status404NotFound)]
        [ProducesResponseType(StatusCodes.Status401Unauthorized)]
        [ProducesResponseType(StatusCodes.Status403Forbidden)]
        [ProducesDefaultResponseType]
        public async Task<ActionResult<CommandResponse<bool>>> SaveStats(Guid id, SaveMatchStatsCommand command)
        {
            command.MatchId = id;
            command.SubmitterClubId = User.IsInRole("Administrator")
                ? null
                : User.FindFirstValue("ClubId");
            var response = await _mediator.Send(command);
            return Ok(response);
        }

        [HttpPatch("{id}/venue", Name = "UpdateMatchVenue")]
        [Authorize(Roles = "Administrator,ClubManager")]
        [ProducesResponseType(typeof(CommandResponse<bool>), StatusCodes.Status200OK)]
        [ProducesResponseType(typeof(CustomProblemDetails), StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status401Unauthorized)]
        [ProducesResponseType(StatusCodes.Status403Forbidden)]
        [ProducesDefaultResponseType]
        public async Task<ActionResult<CommandResponse<bool>>> UpdateVenue(Guid id, UpdateMatchVenueCommand command)
        {
            command.MatchId = id;
            command.RequesterClubId = User.IsInRole("Administrator") ? null : User.FindFirstValue("ClubId");
            var response = await _mediator.Send(command);
            return Ok(response);
        }

        [HttpPost("{id}/submit-home", Name = "SubmitHomeStats")]
        [Authorize(Roles = "Administrator,ClubManager")]
        [ProducesResponseType(typeof(CommandResponse<bool>), StatusCodes.Status200OK)]
        [ProducesResponseType(typeof(CustomProblemDetails), StatusCodes.Status400BadRequest)]
        [ProducesResponseType(typeof(CustomProblemDetails), StatusCodes.Status404NotFound)]
        [ProducesResponseType(StatusCodes.Status401Unauthorized)]
        [ProducesResponseType(StatusCodes.Status403Forbidden)]
        [ProducesDefaultResponseType]
        public async Task<ActionResult<CommandResponse<bool>>> SubmitHome(Guid id)
        {
            var command = new SubmitHomeStatsCommand
            {
                MatchId = id,
                SubmitterClubId = User.IsInRole("Administrator") ? null : User.FindFirstValue("ClubId")
            };
            var response = await _mediator.Send(command);
            return Ok(response);
        }

        [HttpPost("{id}/confirm", Name = "ConfirmMatchResult")]
        [Authorize(Roles = "Administrator,ClubManager")]
        [ProducesResponseType(typeof(CommandResponse<bool>), StatusCodes.Status200OK)]
        [ProducesResponseType(typeof(CustomProblemDetails), StatusCodes.Status400BadRequest)]
        [ProducesResponseType(typeof(CustomProblemDetails), StatusCodes.Status404NotFound)]
        [ProducesResponseType(StatusCodes.Status401Unauthorized)]
        [ProducesResponseType(StatusCodes.Status403Forbidden)]
        [ProducesDefaultResponseType]
        public async Task<ActionResult<CommandResponse<bool>>> Confirm(Guid id)
        {
            bool isAdmin = User.IsInRole("Administrator");
            var command = new ConfirmMatchResultCommand
            {
                MatchId = id,
                ConfirmedByUserId = User.FindFirstValue(ClaimTypes.NameIdentifier),
                IsForced = isAdmin,
                ConfirmerClubId = isAdmin ? null : User.FindFirstValue("ClubId")
            };
            var response = await _mediator.Send(command);
            return Ok(response);
        }

        [HttpPost("{id}/dispute", Name = "DisputeMatchResult")]
        [Authorize(Roles = "ClubManager")]
        [ProducesResponseType(typeof(CommandResponse<bool>), StatusCodes.Status200OK)]
        [ProducesResponseType(typeof(CustomProblemDetails), StatusCodes.Status400BadRequest)]
        [ProducesResponseType(typeof(CustomProblemDetails), StatusCodes.Status404NotFound)]
        [ProducesResponseType(StatusCodes.Status401Unauthorized)]
        [ProducesResponseType(StatusCodes.Status403Forbidden)]
        [ProducesDefaultResponseType]
        public async Task<ActionResult<CommandResponse<bool>>> Dispute(Guid id, DisputeMatchResultCommand command)
        {
            command.MatchId = id;
            command.DisputerClubId = User.FindFirstValue("ClubId");
            var response = await _mediator.Send(command);
            return Ok(response);
        }

        [HttpPost("{id}/reset", Name = "ResetMatchResult")]
        [Authorize(Roles = "Administrator")]
        [ProducesResponseType(typeof(CommandResponse<bool>), StatusCodes.Status200OK)]
        [ProducesResponseType(typeof(CustomProblemDetails), StatusCodes.Status400BadRequest)]
        [ProducesResponseType(typeof(CustomProblemDetails), StatusCodes.Status404NotFound)]
        [ProducesResponseType(StatusCodes.Status401Unauthorized)]
        [ProducesResponseType(StatusCodes.Status403Forbidden)]
        [ProducesDefaultResponseType]
        public async Task<ActionResult<CommandResponse<bool>>> Reset(Guid id)
        {
            var response = await _mediator.Send(new ResetMatchResultCommand { MatchId = id });
            return Ok(response);
        }

        [HttpPost("{id}/forfeit", Name = "RecordForfeit")]
        [Authorize(Roles = "Administrator")]
        [ProducesResponseType(typeof(CommandResponse<bool>), StatusCodes.Status200OK)]
        [ProducesResponseType(typeof(CustomProblemDetails), StatusCodes.Status400BadRequest)]
        [ProducesResponseType(typeof(CustomProblemDetails), StatusCodes.Status404NotFound)]
        [ProducesResponseType(StatusCodes.Status401Unauthorized)]
        [ProducesResponseType(StatusCodes.Status403Forbidden)]
        [ProducesDefaultResponseType]
        public async Task<ActionResult<CommandResponse<bool>>> Forfeit(Guid id, RecordForfeitCommand command)
        {
            command.MatchId = id;
            command.ConfirmedByUserId = User.FindFirstValue(ClaimTypes.NameIdentifier);
            var response = await _mediator.Send(command);
            return Ok(response);
        }

        [HttpPost("{id}/reschedule", Name = "ProposeReschedule")]
        [Authorize(Roles = "ClubManager")]
        [ProducesResponseType(typeof(CommandResponse<bool>), StatusCodes.Status200OK)]
        [ProducesResponseType(typeof(CustomProblemDetails), StatusCodes.Status400BadRequest)]
        [ProducesResponseType(typeof(CustomProblemDetails), StatusCodes.Status404NotFound)]
        [ProducesResponseType(StatusCodes.Status401Unauthorized)]
        [ProducesResponseType(StatusCodes.Status403Forbidden)]
        [ProducesDefaultResponseType]
        public async Task<ActionResult<CommandResponse<bool>>> ProposeReschedule(Guid id, ProposeRescheduleCommand command)
        {
            command.MatchId = id;
            command.ProposerClubId = Guid.Parse(User.FindFirstValue("ClubId")!);
            command.ProposerUserId = User.FindFirstValue(ClaimTypes.NameIdentifier)!;
            var response = await _mediator.Send(command);
            return Ok(response);
        }

        [HttpPost("{id}/reschedule/respond", Name = "RespondToReschedule")]
        [Authorize(Roles = "ClubManager")]
        [ProducesResponseType(typeof(CommandResponse<bool>), StatusCodes.Status200OK)]
        [ProducesResponseType(typeof(CustomProblemDetails), StatusCodes.Status400BadRequest)]
        [ProducesResponseType(typeof(CustomProblemDetails), StatusCodes.Status404NotFound)]
        [ProducesResponseType(StatusCodes.Status401Unauthorized)]
        [ProducesResponseType(StatusCodes.Status403Forbidden)]
        [ProducesDefaultResponseType]
        public async Task<ActionResult<CommandResponse<bool>>> RespondToReschedule(Guid id, RespondToRescheduleCommand command)
        {
            command.MatchId = id;
            command.ResponderClubId = Guid.Parse(User.FindFirstValue("ClubId")!);
            command.ResponderUserId = User.FindFirstValue(ClaimTypes.NameIdentifier)!;
            var response = await _mediator.Send(command);
            return Ok(response);
        }
    }
}
