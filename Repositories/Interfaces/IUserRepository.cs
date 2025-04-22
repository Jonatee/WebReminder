using WebReminder.Entities;

namespace WebReminder.Repositories.Interfaces
{
    public interface IUserRepository
    {
        Task<User> CreateAsync(User user);
        Task<User> UpdateAsync(User user);
        Task<bool> DeleteAsync(User user);
        Task<User?> GetAsync(Guid id);
        Task<User?> GetAsync(string email);
        Task SaveChanges();
        Task<User?> GetByEmailAsync(string email);
        Task<bool> CheckIfUserExist(string email);
    }
}
