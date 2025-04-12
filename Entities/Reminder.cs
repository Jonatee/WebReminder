namespace WebReminder.Entities
{
    public class Reminder
    {
        public Guid Id { get; set; } = Guid.NewGuid();
        public string Title { get; set; } = default!;
        public string Description { get; set; } = default!;
        public DateTime DueDate { get; set; }
        public DateTime DateCreated { get; set; } = DateTime.Now;
        public bool IsDeleted { get; set; } 
        public string ImageUrl { get; set; }
        public DateTime LastModified { get;set; }
        public Guid UserId { get; set; }
        public User? User { get; set; }
    }
}
