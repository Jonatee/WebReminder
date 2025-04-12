using WebReminder.Entities;

namespace WebReminder.Repositories.Interfaces
{
    public interface IUserRepository
    {
        Task<User> CreateAsync(User user);
        Task<bool> DeleteAsync(User user);
        Task<User?> GetAsync(Guid id);
        Task SaveChanges();
        Task<User?> GetByEmailAsync(string email);
        Task<bool> CheckIfUserExist(string email);
    }
}
