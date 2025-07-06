namespace WebsiteScreenshotService;

public class UserContextAccessor(IHttpContextAccessor httpContextAccessor) : IUserContextAccessor
{
    private readonly IHttpContextAccessor _httpContextAccessor = httpContextAccessor;

    public UserContext? TryCurrentUser()
        => _httpContextAccessor.HttpContext?.GetCustomUser();

    public UserContext GetCurrentUser()
        => _httpContextAccessor.HttpContext?.GetCustomUser()
        ?? throw new InvalidOperationException("User context was not set. This may indicate an invalid or missing authentication token.");
}

public interface IUserContextAccessor
{
    public UserContext GetCurrentUser();

    public UserContext? TryCurrentUser();
}