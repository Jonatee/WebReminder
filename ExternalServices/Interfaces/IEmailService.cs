using WebReminder.Models.DTOs;

namespace WebReminder.ExternalServices.Interfaces
{
    public interface IEmailService
    {
        Task<bool> SendReminderEmail(ReminderEmailRequestModel emailRequest);
        Task<bool> SendWelcomeEmail(WelcomeEmailRequestModel emailRequest);
        Task<bool> SendEmailConfirmation(EmailConfirmationRequestModel emailRequest);
        Task<bool> SendEmail(BaseEmailRequestModel emailRequest);
    }
}
