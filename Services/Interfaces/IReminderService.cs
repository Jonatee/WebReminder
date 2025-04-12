using WebReminder.Models.DTOs;

namespace WebReminder.Services.Interfaces
{
    public interface IReminderService
    {
        Task<ReminderResponseModel> CreateReminder(ReminderRequestModel request);
        Task<ReminderResponseModel> UpdateReminder(ReminderRequestModel request);
        Task DeleteReminder(ReminderRequestModel request);
        Task<ReminderResponseModel> GetReminders(ReminderRequestModel request);
        Task<ReminderResponseModel> RestoreReminder(ReminderRequestModel request);
        Task<IEnumerable<ReminderResponseModel>> GetAllReminders();
        Task<IEnumerable<ReminderResponseModel>> BulkCreate(List<ReminderRequestModel> reminderRequests);

    }
}
