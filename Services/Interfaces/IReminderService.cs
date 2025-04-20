using WebReminder.Models.DTOs;

namespace WebReminder.Services.Interfaces
{
    public interface IReminderService
    {
        Task<BaseResponse<ReminderResponseModel>> CreateReminder(ReminderRequestModel request);
        Task<BaseResponse<ReminderResponseModel>> UpdateReminder(ReminderUpdateModel request);
        Task<BaseResponse<bool>> DeleteReminder(Guid id);
        Task<BaseResponse<ReminderResponseModel>> PermanentDeleteReminder(Guid id);
        Task<BaseResponse<ReminderResponseModel>> RestoreDeletedReminder(Guid id);
        Task<BaseResponse<ReminderResponseModel>> GetReminders(ReminderRequestModel request);
        Task<BaseResponse<ReminderResponseModel>> GetReminder(Guid id);
        Task<BaseResponse<IEnumerable<ReminderResponseModel>>> GetAllReminders();
        Task<IEnumerable<ReminderResponseModel>> GetAllDueReminders();
        Task<IEnumerable<ReminderResponseModel>> GetAllDeletedReminders();
        Task<IEnumerable<ReminderResponseModel>> GetSentReminders();
        Task<bool> SendReminderAsync(ReminderEmailRequestModel request);
        Task<IEnumerable<ReminderResponseModel>> BulkCreate(List<ReminderRequestModel> reminderRequests);

    }
}
