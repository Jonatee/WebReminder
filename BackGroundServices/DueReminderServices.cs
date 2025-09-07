namespace WebReminder.BackGroundServices
{
    using Hangfire;
    using WebReminder.Models.DTOs;
    using WebReminder.Repositories.Interfaces;
    using WebReminder.Services.Interfaces;

    /// <summary>
    /// Defines the <see cref="DueReminderServices" />
    /// </summary>
    public class DueReminderServices
    {
        /// <summary>
        /// Defines the _serviceProvider
        /// </summary>
        private readonly IServiceProvider _serviceProvider;
        private readonly IRecurringJobManager _recurringJobs;

        /// <summary>
        /// Initializes a new instance of the <see cref="DueReminderServices"/> class.
        /// </summary>
        /// <param name="serviceProvider">The serviceProvider<see cref="IServiceProvider"/></param>
        public DueReminderServices(IServiceProvider serviceProvider,IRecurringJobManager recurringJobManager)
        {
            _serviceProvider = serviceProvider;
            _recurringJobs = recurringJobManager;
        }

        /// <summary>
        /// The ExecuteAsync
        /// </summary>
        /// <param name="stoppingToken">The stoppingToken<see cref="CancellationToken"/></param>
        /// <returns>The <see cref="Task"/></returns>
        //protected override async Task ExecuteAsync(CancellationToken stoppingToken)
        //{
        //    while (!stoppingToken.IsCancellationRequested)
        //    {
        //        await CheckAndSendRemindersAsync();
        //        await Task.Delay(TimeSpan.FromMinutes(15), stoppingToken);
        //    }
        //}


        /// <summary>
        /// The CheckAndSendRemindersAsync
        /// </summary>
        /// <returns>The <see cref="Task"/></returns>
        public async Task CheckAndSendRemindersAsync()
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
