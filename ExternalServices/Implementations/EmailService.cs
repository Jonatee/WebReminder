using MailKit.Net.Smtp;
using MimeKit;
using Resend;
using WebReminder.ExternalServices.Interfaces;
using WebReminder.Models.DTOs;

namespace WebReminder.ExternalServices.Implementations
{
    public class EmailService : IEmailService
    {
        private readonly IConfiguration _configuration;
        private readonly string smtpServer = "smtp.gmail.com";
        private readonly int smtpPort = 465;
        public EmailService( IConfiguration configuration)
        {
            _configuration = configuration;
        }
        public Task<bool> SendEmailSMTP(BaseEmailRequestModel mailRequest)
        {
            bool result;
            string username = _configuration["Email:Mail"]!;
            string password = _configuration["Email:MailPassword"]!;
            string senderEmail = _configuration["Email:Mail"]!;
            MimeMessage message = new MimeMessage();
            message.From.Add(new MailboxAddress("Remindify", senderEmail));
            message.To.Add(MailboxAddress.Parse(mailRequest.To));
            message.Subject = mailRequest.Subject;
            var builder = new BodyBuilder
            {

                HtmlBody = mailRequest.HtmlBody
            };

            var body = new TextPart("html")
            {
                Text = mailRequest.HtmlBody,

            };
            message.Body = builder.ToMessageBody();

            SmtpClient client = new SmtpClient();
            try
            {
                client.Connect(smtpServer, smtpPort, true);
                client.Authenticate(username, password);
                client.Send(message);
                result = true;
                return Task.FromResult(result);
            }
            catch (Exception ex)
            {
                result = false;

                return Task.FromResult(result);
            }
            finally
            {
                client.Disconnect(true);
                client.Dispose();
            }
        }

        public async Task<bool> SendEmailConfirmation(EmailConfirmationRequestModel emailRequest)
        {
            emailRequest.Subject = "Web Reminder Email Verification Code";
            emailRequest.HtmlBody = $@"
   <div style='font-family:""Segoe UI"", Arial, sans-serif; max-width:500px; margin:0 auto; background:linear-gradient(135deg, #fbeaff 0%, #ffffff 100%); padding:35px; border-radius:16px; border-top:6px solid #910caa; box-shadow:0 5px 20px rgba(0,0,0,0.08);'>
    <div style='text-align:center; margin-bottom:30px;'>
        <div style='display:inline-block; background:#910caa; border-radius:50%; width:70px; height:70px; line-height:70px; text-align:center; margin-bottom:20px; box-shadow:0 4px 10px rgba(145,12,170,0.3);'>
            <img src='https://cdn-icons-png.flaticon.com/512/6357/6357048.png' alt='Verify' style='width:40px; height:40px; vertical-align:middle; filter:brightness(0) invert(1);' />
        </div>
        <h2 style='color:#750c91; font-size:26px; margin:0; font-weight:700; letter-spacing:0.5px;'>Verify Your Email</h2>
        <p style='color:#5b5b5b; font-size:16px; margin:10px 0 0;'>Please use the verification code below</p>
    </div>
    
    <div style='background-color:white; border-radius:12px; padding:25px; text-align:center; box-shadow:0 3px 10px rgba(0,0,0,0.05); border:1px dashed rgba(145,12,170,0.2);'>
        <p style='font-size:15px; color:#3b003b; margin:0 0 15px; font-weight:500;'>Your verification code is:</p>
        
        <div style='background:linear-gradient(to right, rgba(251,234,255,0.5), rgba(208,72,223,0.1), rgba(251,234,255,0.5)); padding:15px; border-radius:8px; margin:0 auto; max-width:240px;'>
            <p style='font-size:32px; letter-spacing:8px; color:#750c91; font-weight:700; margin:0; font-family:monospace, ""Courier New"", Courier;'>{emailRequest.VerificationCode}</p>
        </div>
        
        <p style='font-size:14px; color:#5b5b5b; margin:20px 0 0;'>This code will expire in <span style='color:#910caa; font-weight:600;'>10 minutes</span></p>
    </div>
    
    <div style='margin-top:30px; padding:20px; background-color:rgba(251,234,255,0.5); border-radius:10px; border-left:4px solid #d048df;'>
        <p style='font-size:15px; color:#3b003b; margin:0; line-height:1.6;'>Enter this code in the app to verify your email address and complete your registration.</p>
    </div>
    
    <div style='margin-top:30px; text-align:center; padding-top:20px; border-top:1px solid rgba(145,12,170,0.15);'>
        <p style='font-size:14px; color:#5b5b5b; margin:0;'>Didn't request this code? <a href='#' style='color:#910caa; text-decoration:none; font-weight:600;'>Contact Support</a></p>
    </div>
</div>
        ";

            return await SendEmailSMTP(emailRequest);
        }

        public async Task<bool> SendReminderEmail(ReminderEmailRequestModel emailRequest)
        {
            emailRequest.Subject = $"Reminder: {emailRequest.Title}";
            emailRequest.HtmlBody = $@"
              <div style=""font-family:'Segoe UI', Arial, sans-serif; max-width:600px; margin:0 auto; background:linear-gradient(150deg, #fbeaff 0%, #ffffff 85%); padding:30px; border-radius:12px; border-left:5px solid #910caa; box-shadow:0 4px 15px rgba(145,12,170,0.1);"">
    <div style=""background-color:#910caa; padding:15px 20px; border-radius:8px; margin-bottom:25px; box-shadow:0 4px 8px rgba(145,12,170,0.2);"">
        <h2 style=""color:#ffffff; margin:0; font-size:22px; font-weight:600; letter-spacing:0.5px;"">📌 Task Reminder</h2>
    </div>
    
    <div style=""background-color:white; border-radius:10px; padding:25px; box-shadow:0 2px 8px rgba(0,0,0,0.05);"">
        <h3 style=""margin:0 0 15px; color:#750c91; font-size:24px; font-weight:700; border-bottom:2px solid #d048df; padding-bottom:10px;"">{emailRequest.Title}</h3>
        
        <div style=""background:rgba(251,234,255,0.5); padding:15px; border-radius:8px; margin:20px 0; border-left:3px solid #d048df;"">
            <p style=""color:#3b003b; font-size:16px; line-height:1.6; margin:0;"">{emailRequest.Description}</p>
        </div>
        
        <div style=""display:inline-block; background:linear-gradient(135deg, #d048df 0%, #910caa 100%); padding:3px; border-radius:50px; margin-top:15px;"">
            <div style=""background:white; border-radius:50px; padding:2px;"">
                <p style=""margin:0; padding:8px 20px; font-size:15px; color:#750c91; font-weight:600;"">
                    <span style=""display:inline-block; background:#fbeaff; padding:5px 10px; border-radius:50px; margin-right:8px;"">⏰</span>
                    Due on: <strong style=""color:#910caa; letter-spacing:0.5px;"">{emailRequest.DueDate:MMMM dd, yyyy}</strong>
                </p>
            </div>
        </div>

        {(string.IsNullOrWhiteSpace(emailRequest.ImageUrl)
            ? ""
            : $@"
        <div style=""margin-top:25px; border-radius:10px; overflow:hidden; box-shadow:0 4px 12px rgba(0,0,0,0.1); border:1px solid #f0f0f0;"">
            <div style=""height:8px; background:linear-gradient(90deg, #910caa, #d048df, #f080ff);""></div>
            <img src=""{emailRequest.ImageUrl}"" alt=""Task Image"" style=""width:100%; max-width:100%; display:block;"" />
        </div>")}

    </div>
    
    <div style=""margin-top:25px; text-align:center;"">
        <a href=""#"" style=""display:inline-block; background:linear-gradient(135deg, #d048df 0%, #910caa 100%); color:white; text-decoration:none; padding:12px 30px; border-radius:50px; font-weight:600; font-size:16px; box-shadow:0 4px 8px rgba(145,12,170,0.2);"">View Task Details</a>
    </div>
    
    <div style=""margin-top:25px; padding-top:15px; border-top:1px dashed rgba(145,12,170,0.3); text-align:center;"">
        <p style=""font-size:14px; color:#5b5b5b; margin:0;"">This is an automated reminder from <span style=""color:#910caa; font-weight:600;"">Remindify</span></p>
    </div>
    </div>";

            return await SendEmailSMTP(emailRequest);
        }

        public async Task<bool> SendWelcomeEmail(WelcomeEmailRequestModel emailRequest)
        {
            emailRequest.Subject = "Welcome to the Community!";
            emailRequest.HtmlBody = $@"
           <div style='font-family:""Segoe UI"", Helvetica, Arial, sans-serif; max-width:600px; margin:0 auto; background:linear-gradient(135deg, #fbeaff 0%, #ffffff 100%); padding:30px; border-radius:12px; border-top:5px solid #910caa; box-shadow:0 4px 12px rgba(0,0,0,0.05);'>
    <div style='text-align:center; margin-bottom:25px;'>
        <h1 style='color:#750c91; font-size:28px; margin:0; padding:0; font-weight:700;'>Welcome, {emailRequest.FirstName}!</h1>
        <p style='color:#910caa; font-size:18px; margin:5px 0 0; font-weight:500;'>{emailRequest.LastName}</p>
    </div>

    <div style='background-color:white; border-radius:8px; padding:25px; border-left:4px solid #d048df; margin:20px 0;'>
        <p style='font-size:16px; color:#3b003b; line-height:1.6; margin:0 0 15px;'>We're thrilled to have you on board. Your account has been successfully created.</p>
        <p style='font-size:16px; color:#3b003b; line-height:1.6; margin:0;'>You can now log in and start using all the features of our platform.</p>
    </div>

    <div style='text-align:center; margin-top:25px;'>
        <a href='#' style='display:inline-block; background:#d048df; color:white; text-decoration:none; padding:12px 30px; border-radius:50px; font-weight:600; font-size:16px; box-shadow:0 4px 8px rgba(145,12,170,0.2);'>Get Started</a>
    </div>

    <div style='margin-top:30px; padding-top:20px; border-top:1px solid rgba(145,12,170,0.2); text-align:center;'>
        <p style='font-size:14px; color:#5b5b5b; margin:0;'>Need help? <a href='#' style='color:#910caa; text-decoration:none; font-weight:500;'>Contact our support team</a></p>
    </div>
</div>
        ";

            return await SendEmailSMTP(emailRequest);
        }
    }

}
