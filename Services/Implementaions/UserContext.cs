using System.Security.Claims;
using WebReminder.Services.Interfaces;

namespace WebReminder.Services.Implementaions
{
    public class UserContext : IUserContext
    {
        private readonly IHttpContextAccessor _contextAccessor;

        public UserContext(IHttpContextAccessor contextAccessor)
        {
            _contextAccessor = contextAccessor;
        }

        public Guid UserId =>
            Guid.TryParse(
                _contextAccessor.HttpContext?.User?.FindFirstValue(ClaimTypes.NameIdentifier),
                out var userId)
            ? userId
            : throw new UnauthorizedAccessException("User not authenticated");

        public string UserIpAddress => _contextAccessor.HttpContext.Request.Headers["X-Forwarded-For"].FirstOrDefault() ??_contextAccessor.HttpContext.Connection.RemoteIpAddress.ToString();
    }

}
