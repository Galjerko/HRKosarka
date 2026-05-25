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

namespace HRKošarka.API.Controllers
{
    [Route("api/matches")]
    public class MatchController : BaseController
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
            var clubId = CallerClubGuid;
            var response = await _mediator.Send(new GetPendingActionsQuery
            {
                IsAdmin = IsAdmin,
                ClubId = clubId,
                TeamRepUserId = (!IsAdmin && clubId == null) ? CurrentUserId : null
            });
            return Ok(response);
        }

        [HttpGet("{id}", Name = "GetMatchWithStats")]
        [ProducesResponseType(typeof(QueryResponse<MatchWithStatsDTO>), StatusCodes.Status200OK)]
        [ProducesResponseType(typeof(CustomProblemDetails), StatusCodes.Status404NotFound)]
        [ProducesDefaultResponseType]
        public async Task<ActionResult<QueryResponse<MatchWithStatsDTO>>> Get(Guid id)
            => Ok(await _mediator.Send(new GetMatchWithStatsQuery(id)));

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
            command.SubmitterClubId = CallerClubId;
            command.SubmitterUserId = CallerUserId;
            return Ok(await _mediator.Send(command));
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
            command.RequesterClubId = CallerClubId;
            command.RequesterUserId = CallerUserId;
            return Ok(await _mediator.Send(command));
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
            var command = new SubmitHomeStatsCommand
            {
                MatchId = id,
                SubmitterClubId = CallerClubId,
                SubmitterUserId = CallerUserId
            };
            return Ok(await _mediator.Send(command));
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
            var command = new ConfirmMatchResultCommand
            {
                MatchId = id,
                ConfirmedByUserId = CurrentUserId,
                IsForced = IsAdmin,
                ConfirmerClubId = CallerClubId,
                ConfirmerUserId = CallerUserId
            };
            return Ok(await _mediator.Send(command));
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
            command.DisputerClubId = CallerClubId;
            command.DisputerUserId = CallerUserId;
            return Ok(await _mediator.Send(command));
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
            => Ok(await _mediator.Send(new ResetMatchResultCommand { MatchId = id }));

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
            command.ConfirmedByUserId = CurrentUserId;
            return Ok(await _mediator.Send(command));
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
            command.ProposerClubId = CallerClubGuid;
            command.ProposerUserId = CurrentUserId;
            return Ok(await _mediator.Send(command));
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
            command.ResponderClubId = CallerClubGuid;
            command.ResponderUserId = CurrentUserId;
            return Ok(await _mediator.Send(command));
        }
    }
}
