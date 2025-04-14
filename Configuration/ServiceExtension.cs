using WebReminder.ExternalServices.Implementations;
using WebReminder.ExternalServices.Interfaces;
using WebReminder.Repositories.Implementaions;
using WebReminder.Repositories.Interfaces;
using WebReminder.Services.Implementaions;
using WebReminder.Services.Interfaces;

namespace WebReminder.Configuration
{
    public static class ServiceExtensions
    {
        public static IServiceCollection AddRepositories(this IServiceCollection services)
        {
            return services

                .AddScoped<IReminderRespository, ReminderRepository>()
                .AddScoped<IUserRepository, UserRepository>();
        }
        public static IServiceCollection AddServices(this IServiceCollection services)
        {
            return services

                .AddScoped<IReminderService, ReminderService>()
                .AddScoped<IEmailService, EmailService>()
                .AddScoped<IFileService, FileService>()
                .AddScoped<IUserContext, UserContext>()
                .AddScoped<IUserService, UserService>();
        }
    }
}
