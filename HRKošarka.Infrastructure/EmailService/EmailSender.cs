using HRKošarka.Application.Contracts.Email;
using HRKošarka.Application.Models.Email;
using Microsoft.Extensions.Options;
using System.Net.Mail;

namespace HRKošarka.Infrastructure.EmailService
{
    public class EmailSender : IEmailSender
    {
        public EmailSettings _emailSettings { get; }
        public EmailSender(IOptions<EmailSettings> emailSettings)
        {
            _emailSettings = emailSettings.Value;
        }
        public async Task SendEmail(EmailMessage email)
        {
            var smtpClient = new SmtpClient(_emailSettings.Host, _emailSettings.Port);
            smtpClient.EnableSsl = true;
            smtpClient.UseDefaultCredentials = false;
            smtpClient.Credentials = new System.Net.NetworkCredential(_emailSettings.Email, _emailSettings.Password);


            var message = new MailMessage(_emailSettings.Email, email.To, email.Subject, email.Body);
            await smtpClient.SendMailAsync(message);
        }
    }
}
