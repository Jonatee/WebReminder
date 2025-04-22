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
using WebReminder.ExternalServices.Interfaces;

namespace WebReminder.Services.Implementaions
{
    public class UserService : IUserService
    {
        private readonly IUserRepository _userRepository;
        private readonly IHttpContextAccessor _httpContextAccessor;
        private readonly IEmailService _emailService;

        public UserService(IUserRepository userRepository, IHttpContextAccessor httpContextAccessor,IEmailService emailService)
        {
            _userRepository = userRepository;
            _emailService = emailService;
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
                Id = user.Id,
                IsVerified = user.IsVerified

            };
            return response;
        }

        public async Task<UserResponseModel> GetUser(string email)
        {
            var user = await _userRepository.GetAsync(email);
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
                Id = user.Id,
                IsVerified = user.IsVerified
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
                IsVerified = getUser.IsVerified
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
                var welcomeMail = new WelcomeEmailRequestModel
                {
                    FirstName = createdUser.FirstName,
                    LastName = createdUser.LastName,
                    To = createdUser.Email,
                };
                await _emailService.SendWelcomeEmail(welcomeMail);
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

        public async Task<UserResponseModel> UpdateUser(string email)
        {
            var user = await _userRepository.GetAsync(email);
            user.IsVerified = true;
            var update = await _userRepository.UpdateAsync(user);
            if(update == null)
            {
                return null;
            }
            return new UserResponseModel
            {
                IsVerified = update.IsVerified,
                Email = update.Email,
                FirstName = update.FirstName,
                Id = update.Id,
                LastName = update.LastName
            };
        }
    }
}
