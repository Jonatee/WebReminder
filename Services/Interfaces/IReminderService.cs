using WebReminder.Models.DTOs;

namespace WebReminder.Services.Interfaces
{
    public interface IReminderService
    {
        Task<BaseResponse<ReminderResponseModel>> CreateReminder(ReminderRequestModel request);
        Task<BaseResponse<ReminderResponseModel>> UpdateReminder(ReminderUpdateModel request);
        Task DeleteReminder(ReminderRequestModel request);
        Task<BaseResponse<ReminderResponseModel>> GetReminders(ReminderRequestModel request);
        Task<BaseResponse<ReminderResponseModel>> RestoreReminder(ReminderRequestModel request);
        Task<BaseResponse<IEnumerable<ReminderResponseModel>>> GetAllReminders();
        Task<IEnumerable<ReminderResponseModel>> GetAllDueReminders();
        Task<bool> SendReminderAsync(ReminderEmailRequestModel request);
        Task<IEnumerable<ReminderResponseModel>> BulkCreate(List<ReminderRequestModel> reminderRequests);

    }
}
