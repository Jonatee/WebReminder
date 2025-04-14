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

        public async Task<Reminder> DeleteReminderAsync(Guid reminderId)
        {
            var reminder = await db.Reminders.FirstOrDefaultAsync(x => x.Id == reminderId);
            db.Reminders.Remove(reminder!);
            return null;
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
         return await db.Reminders.FirstOrDefaultAsync(x => x.Title == title && x.Description == description && x.DueDate == date);
        }


        public async Task<Reminder> UpdateReminderAsync(Guid reminderId)
        {
            var reminder = await db.Reminders.FirstOrDefaultAsync(x => x.Id == reminderId);
            db.Reminders.Update(reminder!);
            return reminder;
        }
        public async Task<Reminder> UpdateReminderAsync(Reminder reminder)
        {
            var getReminder = await db.Reminders.FirstOrDefaultAsync(x => x.Id == reminder.Id);
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

        public async Task<List<Reminder>> GetDueRemindersAsync()
        {
            if (!_cache.TryGetValue("DueReminders", out List<Reminder> dueReminders))
            {
                dueReminders = await db.Reminders
                    .Where(r =>
                        !r.IsSent &&
                        r.DueDate <= DateTime.UtcNow &&
                        r.DueDate >= DateTime.UtcNow.AddMinutes(-5) &&
                        r.DueDate <= DateTime.UtcNow.AddMinutes(5)
                    ).ToListAsync();

                var cacheExpirationOptions = new MemoryCacheEntryOptions()
                    .SetSlidingExpiration(TimeSpan.FromMinutes(5))  // Cache will refresh every 5 minutes
                    .SetAbsoluteExpiration(TimeSpan.FromMinutes(10));  // Cache will expire after 10 minutes

                _cache.Set("DueReminders", dueReminders, cacheExpirationOptions);
            }

            return dueReminders;
        }


    }
}
