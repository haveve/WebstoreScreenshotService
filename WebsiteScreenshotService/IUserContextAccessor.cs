namespace WebsiteScreenshotService;

public interface IUserContextAccessor
{
    public UserContext GetCurrentUser();

    public UserContext? TryCurrentUser();
}
