using HRKošarka.API.Models;
using HRKošarka.Application.Features.LeaveType.Commands.CreateLeaveType;
using HRKošarka.Application.Features.LeaveType.Commands.DeleteLeaveType;
using HRKošarka.Application.Features.LeaveType.Commands.UpdateLeaveType;
using HRKošarka.Application.Features.LeaveType.Queries.GetAllLeaveTypes;
using HRKošarka.Application.Features.LeaveType.Queries.GetLeaveTypeDetails;
using HRKošarka.Application.Models.Responses;
using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace HRKošarka.API.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    [Authorize]
    public class LeaveTypesController : ControllerBase
    {
        private readonly IMediator _mediator;

        public LeaveTypesController(IMediator mediator)
        {
            _mediator = mediator;
        }

        [HttpGet(Name = "GetAllLeaveTypes")]
        [ProducesResponseType(typeof(PaginatedResponse<LeaveTypeDTO>), 200)]
        [ProducesResponseType(typeof(PaginatedResponse<LeaveTypeDTO>), 500)]
        public async Task<ActionResult<PaginatedResponse<LeaveTypeDTO>>> GetTest([FromQuery] GetLeaveTypesQuery query)
        {
            var response = await _mediator.Send(query);

            if (response.IsSuccess)
            {
                return Ok(response);
            }

            return StatusCode(500, response);
        }

        [HttpGet("{id}", Name = "GetLeaveTypeById")]
        [ProducesResponseType(typeof(QueryResponse<LeaveTypeDetailsDto>), 200)]
        [ProducesResponseType(typeof(QueryResponse<LeaveTypeDetailsDto>), 404)]
        public async Task<ActionResult<QueryResponse<LeaveTypeDetailsDto>>> Get(Guid id)
        {
            var leaveTypes = await _mediator.Send(new GetLeaveTypeDetailsQuery(id));
            return Ok(leaveTypes);
        }

        [HttpPost(Name = "CreateLeaveType")]
        [ProducesResponseType(typeof(CommandResponse<Guid>), 201)]
        [ProducesResponseType(typeof(CustomProblemDetails), 400)]
        public async Task<ActionResult<CommandResponse<Guid>>> Post(CreateLeaveTypeCommand leaveType)
        {
            var response = await _mediator.Send(leaveType);
            return CreatedAtAction(nameof(Get), new { id = response.Data }, response);
        }

        [HttpPut("{id}", Name = "UpdateLeaveType")]
        [ProducesResponseType(StatusCodes.Status204NoContent)]
        [ProducesResponseType(400)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        [ProducesDefaultResponseType]
        public async Task<ActionResult> Put(UpdateLeaveTypeCommand leaveType)
        {
            await _mediator.Send(leaveType);
            return NoContent();
        }

        [HttpDelete("{id}", Name = "DeleteLeaveType")]
        [ProducesResponseType(StatusCodes.Status204NoContent)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        [ProducesDefaultResponseType]
        public async Task<ActionResult> Delete(Guid id)
        {
            var command = new DeleteLeaveTypeCommand { Id = id };
            await _mediator.Send(command);
            return NoContent();
        }
    }
}
