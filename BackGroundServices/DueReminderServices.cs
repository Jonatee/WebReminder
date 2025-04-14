
using CloudinaryDotNet.Actions;
using WebReminder.Entities;
using WebReminder.Models.DTOs;
using WebReminder.Repositories.Interfaces;
using WebReminder.Services.Interfaces;

namespace WebReminder.BackGroundServices
{
    public class DueReminderServices : BackgroundService
    {
        private readonly IServiceProvider _serviceProvider;
        private readonly IUserRepository _userRepository;
        private readonly IReminderRespository _reminderRepository;


        public DueReminderServices(IServiceProvider serviceProvider,IUserRepository userRepository, IReminderRespository reminderRepository)
        {
            _serviceProvider = serviceProvider;
            _userRepository = userRepository;
            _reminderRepository = reminderRepository;
        }

        protected override async Task ExecuteAsync(CancellationToken stoppingToken)
        {
            while (!stoppingToken.IsCancellationRequested)
            {
                await CheckAndSendRemindersAsync();
                await Task.Delay(TimeSpan.FromMinutes(1), stoppingToken);
            }
        }

        private async Task CheckAndSendRemindersAsync()
        {
            using (var scope = _serviceProvider.CreateScope())
            {
                var reminderService = scope.ServiceProvider.GetRequiredService<IReminderService>();
                var dueReminders = await reminderService.GetAllDueReminders();

                foreach (var reminder in dueReminders)
                {
                    var user =  await _userRepository.GetAsync(reminder.UserId);
                    var sendEmail = new ReminderEmailRequestModel
                    {
                        ReminderId = reminder.Id,
                        DateCreated = reminder.DateCreated,
                        Description = reminder.Description,
                        DueDate = reminder.DueDate,
                        ImageUrl = reminder.ImageUrl,
                        Title = reminder.Title,
                        To = user!.Email,
                    };
                    var emailSent = await reminderService.SendReminderAsync(sendEmail);
                    if (emailSent)
                    {
                        reminder.IsSent = true;
                        var newReminder = new Reminder
                        {
                            UserId = reminder.UserId,
                            Id = reminder.Id,
                            IsSent = reminder.IsSent,
                            DueDate = reminder.DueDate,
                            Description = reminder.Description,
                            DateCreated = reminder.DateCreated,
                            ImageUrl = reminder.ImageUrl,
                            Title = reminder.Title,
                            LastModified = DateTime.Now,
                            IsDeleted = reminder.IsDeleted
                        };
                        await _reminderRepository.UpdateReminderAsync(newReminder);
                    }
                }
            }
        }

    }
}
