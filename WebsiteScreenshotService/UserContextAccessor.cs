using WebsiteScreenshotService.Extensions;

namespace WebsiteScreenshotService;

public class UserContextAccessor(IHttpContextAccessor httpContextAccessor) : IUserContextAccessor
{
    private readonly IHttpContextAccessor _httpContextAccessor = httpContextAccessor;

    public UserContext? TryCurrentUser()
        => _httpContextAccessor.HttpContext?.GetUserContext();

    public UserContext GetCurrentUser()
        => _httpContextAccessor.HttpContext?.GetUserContext()
            ?? ThrowInvalidOperation<UserContext>();

    public UserSpecificServices? TryUserSpecificServices()
    => _httpContextAccessor.HttpContext?.GetUserSpecificServices();

    public UserSpecificServices GetUserSpecificServices()
        => _httpContextAccessor.HttpContext?.GetUserSpecificServices()
            ?? ThrowInvalidOperation<UserSpecificServices>();

    private static T ThrowInvalidOperation<T>()
        => throw new InvalidOperationException($"{typeof(T)} was not set. This may indicate an invalid or missing authentication token.");
}