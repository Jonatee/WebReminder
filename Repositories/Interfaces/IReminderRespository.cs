using WebReminder.Entities;

namespace WebReminder.Repositories.Interfaces
{
    public interface IReminderRespository
    {
        Task<Reminder> AddReminderAsync(Reminder reminder);
        Task<Reminder> UpdateReminderAsync(Guid reminderId);
        Task<Reminder> UpdateReminderAsync(Reminder reminder);
        Task<bool> DeleteReminderAsync(Guid reminderId);
        Task<Reminder> RestoreDeletedReminderAsync(Guid reminderId);
        Task<List<Reminder>> GetDueRemindersAsync();
        Task<List<Reminder>> GetSentRemindersAsync();
        Task<List<Reminder>> GetAllDeletedRemindersAsync();
        Task<Reminder?> GetReminderAsync(Guid reminderId);
        Task<Reminder?> GetReminderAsync(string title,string description);
        Task<Reminder?> GetReminderAsync(string title, string description,DateTime date);
        Task SaveChanges();
        Task AddRangeAsync(IEnumerable<Reminder> reminders);
        Task<IEnumerable<Reminder>> GetAllReminders(Guid userId);
        
    }
}
