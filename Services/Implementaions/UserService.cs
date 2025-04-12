
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Authentication;
using Microsoft.IdentityModel.Tokens;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;
using WebReminder.Entities;
using WebReminder.Models.DTOs;
using WebReminder.Repositories.Interfaces;
using WebReminder.Services.Interfaces;

namespace WebReminder.Services.Implementaions
{
    public class UserService : IUserService
    {
        private readonly IUserRepository _userRepository;
        private readonly IHttpContextAccessor _httpContextAccessor;

        public UserService(IUserRepository userRepository, IHttpContextAccessor httpContextAccessor)
        {
            _userRepository = userRepository;
            _httpContextAccessor = httpContextAccessor;
        }

        public async Task<UserResponseModel> GetUser(Guid id)
        {
            var user = await _userRepository.GetAsync(id);
            if (user == null)
            {
                return null;
            }
            var response = new UserResponseModel
            {
                Email = user.Email,
                FirstName = user.FirstName,
                LastName = user.LastName,
                LastLoginAt = user.LastLoginAt,
                Id = user.Id
            };
            return response;
        }

        public async Task<UserResponseModel> LoginUser(LoginRequestModel request)
        {
            var userExists = await _userRepository.CheckIfUserExist(request.Email);
            if (!userExists)
            {
                return null;
            }

            var getUser = await _userRepository.GetByEmailAsync(request.Email);


            bool checkPassword = BCrypt.Net.BCrypt.Verify(request.Password, getUser.PasswordHash);
            if (!checkPassword)
            {
                return null;
            }
            getUser.LastLoginAt = DateTime.UtcNow;


            var claims = new List<Claim>
            {
                new Claim(ClaimTypes.Name, $"{getUser.FirstName} {getUser.LastName}"),
               new Claim(ClaimTypes.Email, getUser.Email),
              new Claim(ClaimTypes.NameIdentifier, getUser.Id.ToString())
               };

            var identity = new ClaimsIdentity(claims, CookieAuthenticationDefaults.AuthenticationScheme);
            var principal = new ClaimsPrincipal(identity);
            var properties = new AuthenticationProperties();

            await _httpContextAccessor.HttpContext.SignInAsync(CookieAuthenticationDefaults.AuthenticationScheme, principal, properties);

            var userResponse = new UserResponseModel
            {
                Email = getUser.Email,
                FirstName = getUser.FirstName,
                LastName = getUser.LastName,
                LastLoginAt = getUser.LastLoginAt,
                Id = getUser.Id,
            };
            _userRepository.SaveChanges();

            return userResponse;
        }

        public async Task<UserResponseModel> RegisterUser(UserRequestModel request)
        {
            var userExists = await _userRepository.CheckIfUserExist(request.Email);
            if (userExists)
            {
                return null;
            }
            var salt = BCrypt.Net.BCrypt.GenerateSalt();
            var hashedPassword = BCrypt.Net.BCrypt.HashPassword(request.Password, salt);

            var newUser = new User
            {
                FirstName = request.FirstName,
                LastName = request.LastName,
                Email = request.Email,
                PasswordHash = hashedPassword,
            };

            var createdUser = await _userRepository.CreateAsync(newUser);
            if (createdUser != null)
            {
                var claims = new List<Claim>
            {
               new Claim(ClaimTypes.Name, createdUser.FirstName + " " + createdUser.LastName),
               new Claim(ClaimTypes.Email, createdUser.Email),
               new Claim(ClaimTypes.NameIdentifier, createdUser.Id.ToString())
            };
                var identity = new ClaimsIdentity(claims, CookieAuthenticationDefaults.AuthenticationScheme);
                var principal = new ClaimsPrincipal(identity);
                var properties = new AuthenticationProperties();

               await _httpContextAccessor.HttpContext.SignInAsync(CookieAuthenticationDefaults.AuthenticationScheme, principal, properties);

                return new UserResponseModel
                {
                    FirstName = createdUser.FirstName,
                    LastName = createdUser.LastName,
                    Email = createdUser.Email,
                    Id = createdUser.Id
                };  
            }

            return null;
        }

        public async Task<bool> RemoveUser(Guid id)
        {
            var userExists = await _userRepository.GetAsync(id);
            if (userExists is null)
            {
                return false;
            }
            var isDeleted =  await _userRepository.DeleteAsync(userExists);
            if(isDeleted)
            {
                return true;
            }
            return false;
        }
    }
}
