using HRKošarka.Application.Contracts.Email;
using HRKošarka.Application.Features.EmailNotification.Queries.GetEmailNotifications;
using HRKošarka.Application.Models.Email;
using HRKošarka.Application.Models.Responses;
using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace HRKošarka.API.Controllers
{
    [Route("api/email")]
    [ApiController]
    public class EmailController : ControllerBase
    {
        private readonly IEmailSender _emailSender;
        private readonly IMediator _mediator;

        public EmailController(IEmailSender emailSender, IMediator mediator)
        {
            _emailSender = emailSender;
            _mediator = mediator;
        }

        [HttpGet("notifications", Name = "GetEmailNotifications")]
        [Authorize(Roles = "Administrator")]
        [ProducesResponseType(typeof(PaginatedResponse<EmailNotificationDTO>), StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status401Unauthorized)]
        [ProducesResponseType(StatusCodes.Status403Forbidden)]
        [ProducesDefaultResponseType]
        public async Task<ActionResult<PaginatedResponse<EmailNotificationDTO>>> GetNotifications(
            [FromQuery] GetEmailNotificationsQuery query)
        {
            var response = await _mediator.Send(query);
            return Ok(response);
        }

        [HttpPost("send")]
        public async Task<IActionResult> SendEmail([FromBody] EmailMessage email)
        {
            if (email == null || string.IsNullOrEmpty(email.To) || string.IsNullOrEmpty(email.Subject) || string.IsNullOrEmpty(email.Body))
            {
                return BadRequest("Invalid email message.");
            }

            try
            {
                await _emailSender.SendEmail(email);
                return Ok("Email sent successfully.");
            }
            catch (Exception ex)
            {
                return StatusCode(StatusCodes.Status500InternalServerError, $"Error sending email: {ex.Message}");
            }
        }
    }
}
