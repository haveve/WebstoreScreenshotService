namespace WebsiteScreenshotService;

public static class HttpContextExtensions
{
    private const string UserContextKey = "UserContext";

    public static void SetCustomUser(this HttpContext context, UserContext data)
    {
        context.Items[UserContextKey] = data;
    }

    public static UserContext? GetCustomUser(this HttpContext context)
    {
        return context.Items.TryGetValue(UserContextKey, out var value)
            ? value as UserContext
            : null;
    }
}
