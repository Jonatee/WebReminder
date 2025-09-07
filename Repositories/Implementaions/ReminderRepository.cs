using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Caching.Memory;
using System.Collections.Immutable;
using WebReminder.Context;
using WebReminder.Entities;
using WebReminder.Repositories.Interfaces;

namespace WebReminder.Repositories.Implementaions
{
    public class ReminderRepository : IReminderRespository
    {
        private readonly ReminderDb db;
        private readonly IMemoryCache _cache;
        public ReminderRepository(ReminderDb reminderDb, IMemoryCache cache)
        {
            db = reminderDb;
            _cache = cache;
        }

        public async Task<Reminder> AddReminderAsync(Reminder reminder)
        {
            await db.Reminders.AddAsync(reminder);
            await db.SaveChangesAsync();
            return reminder;
        }

        public async Task<bool> DeleteReminderAsync(Guid reminderId)
        {
            var reminder = await db.Reminders.FirstOrDefaultAsync(x => x.Id == reminderId);
            if (reminder is not null)
            {
                db.Reminders.Remove(reminder!);
                await db.SaveChangesAsync();
                return true;
            }
            return false;
        }

        public async Task<IEnumerable<Reminder>> GetAllReminders(Guid userId)
        {
            var result = await db.Reminders
                .Where(x => x.UserId == userId && !x.IsDeleted && !x.IsSent)
                .OrderByDescending(x => x.DueDate)
                .ToListAsync();

            return result;
        }


        public async Task<Reminder?> GetReminderAsync(Guid reminderId)
        {
           return await db.Reminders.FirstOrDefaultAsync(x => x.Id == reminderId);
        }
        public async Task<Reminder?> GetReminderAsync(string title,string description)
        {
            return await db.Reminders.FirstOrDefaultAsync(x => x.Title == title && x.Description == description);
        }
        public async Task<Reminder?> GetReminderAsync(string title, string description, DateTime date)
        {
            var utcDate = DateTime.SpecifyKind(date, DateTimeKind.Unspecified).ToUniversalTime();
            return await db.Reminders.FirstOrDefaultAsync(x =>
                x.Title == title &&
                x.Description == description &&
                x.DueDate == utcDate);
        }



        public async Task<Reminder> UpdateReminderAsync(Guid reminderId)
        {
            var reminder = await db.Reminders.FirstOrDefaultAsync(x => x.Id == reminderId);
            db.Reminders.Update(reminder!);
            await db.SaveChangesAsync();
            return reminder;
        }
        public async Task<Reminder> UpdateReminderAsync(Reminder reminder)
        {
            var getReminder = await db.Reminders.FirstOrDefaultAsync(x => x.Id == reminder.Id);
            if (getReminder is null) return null;
            db.Reminders.Update(reminder!);
            await db.SaveChangesAsync();
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

        public async Task<List<Reminder>> GetDueRemindersAsync()
        {
           
            var now = DateTime.UtcNow;
            var start = now.AddMinutes(-30);
            var end = now.AddMinutes(30);

            var dueReminders = await db.Reminders
                .Where(r => !r.IsSent && !r.IsDeleted && r.DueDate >= start && r.DueDate <= end)
                .ToListAsync();
            return dueReminders;
        }

        public async Task<List<Reminder>> GetSentRemindersAsync()
        {
            var dueReminders = await db.Reminders
                 .Where(r => r.IsSent).OrderByDescending(r =>r.LastModified)
                 .ToListAsync();
            return dueReminders;
        }

        public async Task<List<Reminder>> GetAllDeletedRemindersAsync()
        {
            var dueReminders = await db.Reminders
                 .Where(r => r.IsDeleted == true).OrderByDescending(r => r.LastModified)
                 .ToListAsync();
            return dueReminders;
        }

        public async Task<Reminder> RestoreDeletedReminderAsync(Guid reminderId)
        {
            var getReminder = await db.Reminders.FirstOrDefaultAsync(x => x.Id == reminderId);
            if (getReminder is null)
                return null;
            if (DateTime.UtcNow > getReminder.DueDate)
                return null;
            getReminder.IsDeleted = false;
            db.Reminders.Update(getReminder);
            await db.SaveChangesAsync();
            return getReminder;
        }
    }
}
