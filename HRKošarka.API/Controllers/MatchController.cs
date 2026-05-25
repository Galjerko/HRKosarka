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
            string? teamRepUserId = null;
            if (!isAdmin)
            {
                if (User.IsInRole("ClubManager"))
                {
                    var raw = User.FindFirstValue("ClubId");
                    if (!string.IsNullOrEmpty(raw) && Guid.TryParse(raw, out var parsed))
                        clubId = parsed;
                }
                else
                {
                    teamRepUserId = User.FindFirstValue("uid");
                }
            }
            var response = await _mediator.Send(new GetPendingActionsQuery
            {
                ClubId = clubId,
                IsAdmin = isAdmin,
                TeamRepUserId = teamRepUserId
            });
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
        [Authorize]
        [ProducesResponseType(typeof(CommandResponse<bool>), StatusCodes.Status200OK)]
        [ProducesResponseType(typeof(CustomProblemDetails), StatusCodes.Status400BadRequest)]
        [ProducesResponseType(typeof(CustomProblemDetails), StatusCodes.Status404NotFound)]
        [ProducesResponseType(StatusCodes.Status401Unauthorized)]
        [ProducesResponseType(StatusCodes.Status403Forbidden)]
        [ProducesDefaultResponseType]
        public async Task<ActionResult<CommandResponse<bool>>> SaveStats(Guid id, SaveMatchStatsCommand command)
        {
            command.MatchId = id;
            bool isAdmin = User.IsInRole("Administrator");
            command.SubmitterClubId = isAdmin ? null : User.FindFirstValue("ClubId");
            command.SubmitterUserId = isAdmin ? null : User.FindFirstValue("uid");
            var response = await _mediator.Send(command);
            return Ok(response);
        }

        [HttpPatch("{id}/venue", Name = "UpdateMatchVenue")]
        [Authorize]
        [ProducesResponseType(typeof(CommandResponse<bool>), StatusCodes.Status200OK)]
        [ProducesResponseType(typeof(CustomProblemDetails), StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status401Unauthorized)]
        [ProducesResponseType(StatusCodes.Status403Forbidden)]
        [ProducesDefaultResponseType]
        public async Task<ActionResult<CommandResponse<bool>>> UpdateVenue(Guid id, UpdateMatchVenueCommand command)
        {
            command.MatchId = id;
            bool isAdmin = User.IsInRole("Administrator");
            command.RequesterClubId = isAdmin ? null : User.FindFirstValue("ClubId");
            command.RequesterUserId = isAdmin ? null : User.FindFirstValue("uid");
            var response = await _mediator.Send(command);
            return Ok(response);
        }

        [HttpPost("{id}/submit-home", Name = "SubmitHomeStats")]
        [Authorize]
        [ProducesResponseType(typeof(CommandResponse<bool>), StatusCodes.Status200OK)]
        [ProducesResponseType(typeof(CustomProblemDetails), StatusCodes.Status400BadRequest)]
        [ProducesResponseType(typeof(CustomProblemDetails), StatusCodes.Status404NotFound)]
        [ProducesResponseType(StatusCodes.Status401Unauthorized)]
        [ProducesResponseType(StatusCodes.Status403Forbidden)]
        [ProducesDefaultResponseType]
        public async Task<ActionResult<CommandResponse<bool>>> SubmitHome(Guid id)
        {
            bool isAdmin = User.IsInRole("Administrator");
            var command = new SubmitHomeStatsCommand
            {
                MatchId = id,
                SubmitterClubId = isAdmin ? null : User.FindFirstValue("ClubId"),
                SubmitterUserId = isAdmin ? null : User.FindFirstValue("uid")
            };
            var response = await _mediator.Send(command);
            return Ok(response);
        }

        [HttpPost("{id}/confirm", Name = "ConfirmMatchResult")]
        [Authorize]
        [ProducesResponseType(typeof(CommandResponse<bool>), StatusCodes.Status200OK)]
        [ProducesResponseType(typeof(CustomProblemDetails), StatusCodes.Status400BadRequest)]
        [ProducesResponseType(typeof(CustomProblemDetails), StatusCodes.Status404NotFound)]
        [ProducesResponseType(StatusCodes.Status401Unauthorized)]
        [ProducesResponseType(StatusCodes.Status403Forbidden)]
        [ProducesDefaultResponseType]
        public async Task<ActionResult<CommandResponse<bool>>> Confirm(Guid id)
        {
            bool isAdmin = User.IsInRole("Administrator");
            var userId = User.FindFirstValue("uid");
            var command = new ConfirmMatchResultCommand
            {
                MatchId = id,
                ConfirmedByUserId = User.FindFirstValue(ClaimTypes.NameIdentifier),
                IsForced = isAdmin,
                ConfirmerClubId = isAdmin ? null : User.FindFirstValue("ClubId"),
                ConfirmerUserId = isAdmin ? null : userId
            };
            var response = await _mediator.Send(command);
            return Ok(response);
        }

        [HttpPost("{id}/dispute", Name = "DisputeMatchResult")]
        [Authorize]
        [ProducesResponseType(typeof(CommandResponse<bool>), StatusCodes.Status200OK)]
        [ProducesResponseType(typeof(CustomProblemDetails), StatusCodes.Status400BadRequest)]
        [ProducesResponseType(typeof(CustomProblemDetails), StatusCodes.Status404NotFound)]
        [ProducesResponseType(StatusCodes.Status401Unauthorized)]
        [ProducesResponseType(StatusCodes.Status403Forbidden)]
        [ProducesDefaultResponseType]
        public async Task<ActionResult<CommandResponse<bool>>> Dispute(Guid id, DisputeMatchResultCommand command)
        {
            command.MatchId = id;
            bool isAdmin = User.IsInRole("Administrator");
            command.DisputerClubId = isAdmin ? null : User.FindFirstValue("ClubId");
            command.DisputerUserId = isAdmin ? null : User.FindFirstValue("uid");
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
        [Authorize]
        [ProducesResponseType(typeof(CommandResponse<bool>), StatusCodes.Status200OK)]
        [ProducesResponseType(typeof(CustomProblemDetails), StatusCodes.Status400BadRequest)]
        [ProducesResponseType(typeof(CustomProblemDetails), StatusCodes.Status404NotFound)]
        [ProducesResponseType(StatusCodes.Status401Unauthorized)]
        [ProducesResponseType(StatusCodes.Status403Forbidden)]
        [ProducesDefaultResponseType]
        public async Task<ActionResult<CommandResponse<bool>>> ProposeReschedule(Guid id, ProposeRescheduleCommand command)
        {
            command.MatchId = id;
            command.ProposerClubId = Guid.TryParse(User.FindFirstValue("ClubId"), out var clubId) ? clubId : (Guid?)null;
            command.ProposerUserId = User.FindFirstValue("uid") ?? string.Empty;
            var response = await _mediator.Send(command);
            return Ok(response);
        }

        [HttpPost("{id}/reschedule/respond", Name = "RespondToReschedule")]
        [Authorize]
        [ProducesResponseType(typeof(CommandResponse<bool>), StatusCodes.Status200OK)]
        [ProducesResponseType(typeof(CustomProblemDetails), StatusCodes.Status400BadRequest)]
        [ProducesResponseType(typeof(CustomProblemDetails), StatusCodes.Status404NotFound)]
        [ProducesResponseType(StatusCodes.Status401Unauthorized)]
        [ProducesResponseType(StatusCodes.Status403Forbidden)]
        [ProducesDefaultResponseType]
        public async Task<ActionResult<CommandResponse<bool>>> RespondToReschedule(Guid id, RespondToRescheduleCommand command)
        {
            command.MatchId = id;
            command.ResponderClubId = Guid.TryParse(User.FindFirstValue("ClubId"), out var clubId) ? clubId : (Guid?)null;
            command.ResponderUserId = User.FindFirstValue("uid") ?? string.Empty;
            var response = await _mediator.Send(command);
            return Ok(response);
        }
    }
}
