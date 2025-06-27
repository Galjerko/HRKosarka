using HRKošarka.Application.Models.Email;

namespace HRKošarka.Application.Contracts.Email
{
    public interface IEmailSender
    {
        Task SendEmail(EmailMessage email);
    }
}
 