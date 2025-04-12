using Microsoft.EntityFrameworkCore;
using WebReminder.Entities;

namespace WebReminder.Context
{
    public class ReminderDb : DbContext
    {
        public ReminderDb(DbContextOptions<ReminderDb> options) : base(options) { }
        public DbSet<User> Users => Set<User>();
        public DbSet<Reminder> Reminders => Set<Reminder>();
    }
}
