using Microsoft.EntityFrameworkCore;
using WebReminder.Context;
using WebReminder.Entities;
using WebReminder.Repositories.Interfaces;

namespace WebReminder.Repositories.Implementaions
{
    public class ReminderRepository : IReminderRespository
    {
        private readonly ReminderDb db;
        public ReminderRepository(ReminderDb reminderDb) 
        { 
            db = reminderDb;
        }

        public async Task<Reminder> AddReminderAsync(Reminder reminder)
        {
            await db.Reminders.AddAsync(reminder);
            await db.SaveChangesAsync();
            return reminder;
        }

        public async Task<Reminder> DeleteReminderAsync(Guid reminderId)
        {
            var reminder = await db.Reminders.FirstOrDefaultAsync(x => x.Id == reminderId);
            db.Reminders.Remove(reminder!);
            return null;
        }

        public async Task<IEnumerable<Reminder>> GetAllReminders(Guid userId)
        {
            var result =  db.Reminders.AsQueryable();
            await result.Where(x => x.UserId == userId)
                .OrderByDescending(x => x.DueDate)
                .ToListAsync();
            return result; 
        }

        public async Task<Reminder?> GetReminderAsync(Guid reminderId)
        {
           return await db.Reminders.FirstOrDefaultAsync(x => x.Id == reminderId);
        }

        public async Task<Reminder> UpdateReminderAsync(Guid reminderId)
        {
            var reminder = await db.Reminders.FirstOrDefaultAsync(x => x.Id == reminderId);
            db.Reminders.Update(reminder!);
            return reminder;
        }
        public async Task SaveChanges()
        {
            await db.SaveChangesAsync();
        }

        public async Task AddRangeAsync(IEnumerable<Reminder> reminders)
        {
            await db.Reminders.AddRangeAsync(reminders);
        }
    }
}
