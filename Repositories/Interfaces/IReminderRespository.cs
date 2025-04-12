using WebReminder.Entities;

namespace WebReminder.Repositories.Interfaces
{
    public interface IReminderRespository
    {
        Task<Reminder> AddReminderAsync(Reminder reminder);
        Task<Reminder> UpdateReminderAsync(Guid reminderId);
        Task<Reminder> DeleteReminderAsync(Guid reminderId);
        Task<Reminder?> GetReminderAsync(Guid reminderId);
        Task SaveChanges();
        Task AddRangeAsync(IEnumerable<Reminder> reminders);
        Task<IEnumerable<Reminder>> GetAllReminders(Guid userId);
        
    }
}
