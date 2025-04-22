using Microsoft.EntityFrameworkCore;
using WebReminder.Context;
using WebReminder.Entities;
using WebReminder.Repositories.Interfaces;

namespace WebReminder.Repositories.Implementaions
{
    public class UserRepository : IUserRepository
    {
        private readonly ReminderDb _reminderContext;

        public UserRepository(ReminderDb reminderDb)
        {
            _reminderContext = reminderDb;
        }
        public async Task<bool> CheckIfUserExist(string email)
        {
            return await _reminderContext.Users.AnyAsync(x => x.Email == email);
        }

        public async Task<User> CreateAsync(User user)
        {
            await _reminderContext.Users.AddAsync(user);
            await _reminderContext.SaveChangesAsync();
            return user;
        }

        public async Task<bool> DeleteAsync(User user)
        {
           var getUser = await _reminderContext.Users.FirstOrDefaultAsync(x => x.Id == user.Id);
            if (getUser != null)
            {
                var delete = _reminderContext.Users.Remove(getUser);
                await _reminderContext.SaveChangesAsync();
                return true;
            }
            return false;
        }

        public async Task<User?> GetAsync(Guid id)
        {
            return await _reminderContext.Users.FirstOrDefaultAsync(x => x.Id == id);
        }

        public async Task<User?> GetAsync(string email)
        {
            return await _reminderContext.Users.FirstOrDefaultAsync(x => x.Email == email);
        }

        public async Task<User?> GetByEmailAsync(string email)
        {
            return await _reminderContext.Users.FirstOrDefaultAsync(x => x.Email == email);
        }
        public async Task SaveChanges()
        {
            await _reminderContext.SaveChangesAsync();
        }

        public async Task<User> UpdateAsync(User user)
        {
            var getReminder = await _reminderContext.Users.FirstOrDefaultAsync(x => x.Id == user.Id);
            if (getReminder is null) return null;
            _reminderContext.Users.Update(user!);
            await _reminderContext.SaveChangesAsync();
            return user;
        }
    }
}
