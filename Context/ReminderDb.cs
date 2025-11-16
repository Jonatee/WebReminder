using Microsoft.EntityFrameworkCore;
using WebReminder.Entities;
using WebReminder.Models.DTOs;

namespace WebReminder.Context
{
    public class ReminderDb : DbContext
    {
        public ReminderDb(DbContextOptions<ReminderDb> options) : base(options) { }
        public DbSet<User> Users => Set<User>();
        public DbSet<Reminder> Reminders => Set<Reminder>();
        public DbSet<WebReminder.Models.DTOs.UserResponseModel> UserResponseModel { get; set; } = default!;
    }
}
