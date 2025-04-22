using System.ComponentModel.DataAnnotations;

namespace WebReminder.Models.DTOs
{
   
        public class ReminderRequestModel
        {
        [Required]
        [StringLength(100)]
            public string Title { get; set; } = default!;
        [Required]
        [StringLength(100)]
            public string Description { get; set; } = default!;
            public DateTime DueDate { get; set; }
            public IFormFile? Image { get; set; }
        }
           public class ReminderUpdateModel
           {
        public Guid ReminderId { get; set; }
        [Required]
        [StringLength(100)]
        public string Title { get; set; } = default!;
        [Required]
        [StringLength(100)]
        public string Description { get; set; } = default!;
        public DateTime DueDate { get; set; }
        public IFormFile? Image { get; set; }
           }
    public class ReminderResponseModel
        {
            public Guid Id { get; set; }
            public string Title { get; set; } = default!;
            public string Description { get; set; } = default!;
            public DateTime DueDate { get; set; }
            public DateTime DateCreated { get; set; } = DateTime.Now;
            public bool IsDeleted { get; set; }
            public bool IsSent { get; set; }
            public string? ImageUrl { get; set; }
            public DateTime LastModified { get; set; }
            public Guid UserId { get; set; }
        }
}
