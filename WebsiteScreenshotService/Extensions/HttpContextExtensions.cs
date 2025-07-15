namespace WebsiteScreenshotService.Extensions;

public static class HttpContextExtensions
{
    private const string UserContextKey = "UserContext";

    public static void SetUserContext(this HttpContext context, UserContext data)
    {
        context.Items[UserContextKey] = data;
    }

    public static UserContext? GetUserContext(this HttpContext context)
    {
        return context.Items.TryGetValue(UserContextKey, out var value)
            ? value as UserContext
            : null;
    }
}
