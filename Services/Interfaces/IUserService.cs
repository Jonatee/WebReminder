using WebReminder.Models.DTOs;

namespace WebReminder.Services.Interfaces
{
    public interface IUserService
    {
        Task<UserResponseModel> GetUser(Guid id);
        Task<UserResponseModel> UpdateUser(string email);
        Task<UserResponseModel> GetUser(string email);
        Task<bool> RemoveUser(Guid id);
        Task<UserResponseModel> RegisterUser(UserRequestModel request);
        Task<UserResponseModel> LoginUser(LoginRequestModel request);
    }
}
