using WebsiteScreenshotService.Extensions;

namespace WebsiteScreenshotService;

public class UserContextAccessor(IHttpContextAccessor httpContextAccessor) : IUserContextAccessor
{
    private readonly IHttpContextAccessor _httpContextAccessor = httpContextAccessor;

    public UserContext? TryCurrentUser()
        => _httpContextAccessor.HttpContext?.GetUserContext();

    public UserContext GetCurrentUser()
        => _httpContextAccessor.HttpContext?.GetUserContext()
        ?? throw new InvalidOperationException("User context was not set. This may indicate an invalid or missing authentication token.");
}