namespace WebReminder.Entities
{
    public class User
    {
        public Guid Id { get; set; } = Guid.NewGuid();
        public string FirstName { get; set; } = default!;
        public string LastName { get; set; } = default!;
        public string Email { get; set; } = default!;
        public string PasswordHash { get; set; } = default!;
        public HashSet<Reminder> Reminders { get; set; } = new HashSet<Reminder>();
        public DateTime DateCreated { get; set; } = DateTime.UtcNow;
        public DateTime LastLoginAt { get; set; }
        public bool IsVerified{ get; set; }
    }
}
