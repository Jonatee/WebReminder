using Resend;
using WebReminder.ExternalServices.Interfaces;
using WebReminder.Models.DTOs;

namespace WebReminder.ExternalServices.Implementations
{
    public class EmailService : IEmailService
    {
        private readonly IResend _resend;
        private readonly IConfiguration _configuration;


        public EmailService(IResend resend, IConfiguration configuration)
        {
            _resend = resend;
            _configuration = configuration;
        }

        public async Task<bool> SendEmail(BaseEmailRequestModel emailRequest)
        {
            var message = new EmailMessage
            {
                From = _configuration["Resend:FromEmail"]!,
                To = { emailRequest.To },
                Subject = emailRequest.Subject,
                TextBody = emailRequest.TextBody,
                HtmlBody = emailRequest.HtmlBody
            };

            var response = await _resend.EmailSendAsync(message);
            return response != null && response.Success;
        }

        public async Task<bool> SendEmailConfirmation(EmailConfirmationRequestModel emailRequest)
        {
            emailRequest.Subject = "Web Reminder Email Verification Code";
            emailRequest.HtmlBody = $@"
            <div style='font-family:Arial,sans-serif;background:#f9f9f9;padding:20px;border-radius:10px;'>
                <h2 style='color:#333;'>Verify your email</h2>
                <p style='font-size:16px;color:#555;'>Your verification code is:</p>
                <p style='font-size:24px;color:#000;font-weight:bold;'>{emailRequest.VerificationCode}</p>
                <p style='font-size:14px;color:#999;'>Enter this code in the app to complete your registration.</p>
            </div>
        ";

            return await SendEmail(emailRequest);
        }

        public async Task<bool> SendReminderEmail(ReminderEmailRequestModel emailRequest)
        {
            emailRequest.Subject = $"Reminder: {emailRequest.Title}";
            emailRequest.HtmlBody = $@"
            <div style='font-family:Arial,sans-serif;background:#f4f6f8;padding:20px;border-radius:8px;'>
                <h2 style='color:#1a73e8;'>Task Reminder</h2>
                <h3 style='margin-top:10px;color:#333;'>{emailRequest.Title}</h3>
                <p style='color:#555;font-size:16px;'>{emailRequest.Description}</p>
                <p style='font-size:14px;color:#999;'>Due on: <strong>{emailRequest.DueDate:MMMM dd, yyyy}</strong></p>
                {(string.IsNullOrWhiteSpace(emailRequest.ImageUrl) ? "" : $"<img src='{emailRequest.ImageUrl}' alt='Task Image' style='margin-top:15px;width:100%;max-width:400px;border-radius:5px;' />")}
            </div>
        ";

            return await SendEmail(emailRequest);
        }

        public async Task<bool> SendWelcomeEmail(WelcomeEmailRequestModel emailRequest)
        {
            emailRequest.Subject = "Welcome to the Community!";
            emailRequest.HtmlBody = $@"
            <div style='font-family:Segoe UI, sans-serif;background:#ffffff;padding:25px;border:1px solid #e0e0e0;border-radius:10px;'>
                <h1 style='color:#4CAF50;'>Welcome, {emailRequest.FirstName}!</h1>
                <p style='font-size:16px;color:#444;'>We’re thrilled to have you on board. Explore, share, and enjoy the community!</p>
                <p style='font-size:14px;color:#888;'>You can now log in and start using all the features.</p>
            </div>
        ";

            return await SendEmail(emailRequest);
        }
    }

}
