using Microsoft.EntityFrameworkCore;
using System.ComponentModel.DataAnnotations;
namespace WebReminder.Models.DTOs
{
    public class BaseEmailRequestModel
    {
        public string To { get; set; } 
        
        public string Subject { get; set; }
        public string TextBody { get; set; }
        public string HtmlBody { get; set; }
    }
    public class ReminderEmailRequestModel : BaseEmailRequestModel
    {
        public Guid ReminderId { get; set; }
        public string Title { get; set; } = default!;
        public string Description { get; set; } = default!;
        public DateTime DueDate { get; set; }
        public DateTime DateCreated { get; set; }
        public string ImageUrl { get; set; } = string.Empty;
    }
    public class WelcomeEmailRequestModel : BaseEmailRequestModel
    {
        public required string FirstName { get; set; }
        public required string LastName { get; set; }

    }
    public class EmailConfirmationRequestModel : BaseEmailRequestModel
    {
        public required string VerificationCode { get; set; } 
    }

}
