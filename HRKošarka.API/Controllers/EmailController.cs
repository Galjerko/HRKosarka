using HRKošarka.Application.Contracts.Email;
using HRKošarka.Application.Models.Email;
using Microsoft.AspNetCore.Mvc;

namespace HRKošarka.API.Controllers
{
    [Route("api/email")]
    [ApiController]
    public class EmailController : ControllerBase
    {
        private readonly IEmailSender _emailSender;
        public EmailController(IEmailSender emailSender)
        {
            _emailSender = emailSender;
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
