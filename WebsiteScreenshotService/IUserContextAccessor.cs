namespace WebsiteScreenshotService;

public interface IUserContextAccessor
{
    public UserContext GetCurrentUser();

    public UserContext? TryCurrentUser();

    public UserSpecificServices? TryUserSpecificServices();

    public UserSpecificServices GetUserSpecificServices();
}
