namespace WebReminder.Models.DTOs
{
   
        public class ReminderRequestModel
        {
            public string Title { get; set; } = default!;
            public string Description { get; set; } = default!;
            public DateTime DueDate { get; set; }
            public IFormFile? ImageUrl { get; set; }
        }
        public class ReminderResponseModel
        {
            public Guid Id { get; set; }
            public string Title { get; set; } = default!;
            public string Description { get; set; } = default!;
            public DateTime DueDate { get; set; }
            public DateTime DateCreated { get; set; } = DateTime.Now;
            public bool IsDeleted { get; set; }
            public string? ImageUrl { get; set; }
            public DateTime LastModified { get; set; }
            public Guid UserId { get; set; }
        }
}
