
using CloudinaryDotNet.Actions;
using Npgsql;
using WebReminder.Entities;
using WebReminder.Models.DTOs;
using WebReminder.Repositories.Interfaces;
using WebReminder.Services.Interfaces;

namespace WebReminder.BackGroundServices
{
    public class DueReminderServices : BackgroundService
    {
        private readonly IServiceProvider _serviceProvider;

        public DueReminderServices(IServiceProvider serviceProvider)
        {
            _serviceProvider = serviceProvider;
        }

        protected override async Task ExecuteAsync(CancellationToken stoppingToken)
        {
            while (!stoppingToken.IsCancellationRequested)
            {
                await CheckAndSendRemindersAsync();
                await Task.Delay(TimeSpan.FromMinutes(5), stoppingToken);
            }
        }

        private async Task CheckAndSendRemindersAsync()
         {
            using var scope = _serviceProvider.CreateScope();

            var reminderService = scope.ServiceProvider.GetRequiredService<IReminderService>();
            var userRepository = scope.ServiceProvider.GetRequiredService<IUserRepository>();
            var reminderRepository = scope.ServiceProvider.GetRequiredService<IReminderRespository>();

            var dueReminders = await reminderService.GetAllDueReminders();
            if (dueReminders is not null)
            {

                foreach (var reminder in dueReminders)
                {
                    var user = await userRepository.GetAsync(reminder.UserId);
                    if (user is null) continue;

                    var sendEmail = new ReminderEmailRequestModel
                    {
                        ReminderId = reminder.Id,
                        DateCreated = reminder.DateCreated,
                        Description = reminder.Description,
                        DueDate = reminder.DueDate,
                        ImageUrl = reminder.ImageUrl,
                        Title = reminder.Title,
                        To = user.Email,
                    };

                    var emailSent = await reminderService.SendReminderAsync(sendEmail);

                    if (emailSent)
                    {
                        reminder.IsSent = true;

                        reminder.LastModified = DateTime.UtcNow;

                        await reminderRepository.SaveChanges();
                    }
                }
            }
        }
    }
}



