using System.ComponentModel.DataAnnotations;
namespace WebReminder.Models.DTOs
{
    public class BaseEmailRequestModel
    {
        [Required]
        public string To { get; set; }
        [Required]
        public string Subject { get; set; }
        [Required]
        public string TextBody { get; set; }
        [Required]
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
        [Required]
        public string FirstName { get; set; } = default!;
    }
    public class EmailConfirmationRequestModel : BaseEmailRequestModel
    {
        [Required]
        public string VerificationCode { get; set; } = default!;
    }

}
