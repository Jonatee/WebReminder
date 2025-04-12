using Supabase.Gotrue;
using System.Security.Claims;
using WebReminder.Entities;
using WebReminder.Models.DTOs;
using WebReminder.Repositories.Implementaions;
using WebReminder.Repositories.Interfaces;
using WebReminder.Services.Interfaces;

namespace WebReminder.Services.Implementaions
{
    public class ReminderService : IReminderService
    {
        private readonly IReminderRespository _reminderRespository;
        private readonly IUserContext _context;
        private readonly IHttpContextAccessor _contextAccessor;
        public ReminderService(IReminderRespository reminderRespository,IHttpContextAccessor httpContextAccessor, IUserContext context)
        {
            _reminderRespository = reminderRespository;
            _contextAccessor = httpContextAccessor;
            _context = context;
        }
        public async Task<IEnumerable<ReminderResponseModel>> BulkCreate(List<ReminderRequestModel> reminderRequests)
        {
            if (reminderRequests == null || !reminderRequests.Any())
                return Enumerable.Empty<ReminderResponseModel>();
            var userId = _context.UserId;

            var reminders = reminderRequests
                .Where(r => !string.IsNullOrWhiteSpace(r.Title))
                .Select(r => new Reminder
                {
                    DueDate = r.DueDate,
                    Title = r.Title,
                    Description = r.Description,
                    UserId = userId,
                    LastModified = DateTime.UtcNow
                })
                .ToList();

            if (!reminders.Any())
                return Enumerable.Empty<ReminderResponseModel>();

            await _reminderRespository.AddRangeAsync(reminders);
            await _reminderRespository.SaveChanges();

            var response = reminders.Select(r => new ReminderResponseModel
            {
                Id = r.Id,
                Title = r.Title,
                Description = r.Description,
                DueDate = r.DueDate
            });

            return response;
        }


        public async Task<ReminderResponseModel> CreateReminder(ReminderRequestModel request)
        {
            throw new NotImplementedException();
        }

        public async Task DeleteReminder(ReminderRequestModel request)
        {
            throw new NotImplementedException();
        }

        public async Task<IEnumerable<ReminderResponseModel>> GetAllReminders()
        {
            throw new NotImplementedException();
        }

        public async Task<ReminderResponseModel> GetReminders(ReminderRequestModel request)
        {
            throw new NotImplementedException();
        }

        public async Task<ReminderResponseModel> RestoreReminder(ReminderRequestModel request)
        {
            throw new NotImplementedException();
        }

        public async Task<ReminderResponseModel> UpdateReminder(ReminderRequestModel request)
        {
            throw new NotImplementedException();
        }
    }
}
