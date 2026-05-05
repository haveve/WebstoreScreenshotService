namespace WebsiteScreenshotService.Extensions;

public static class HttpContextExtensions
{
    private const string RawAuthToken = "RawAuthToken";
    private const string UserContextKey = "UserContext";
    private const string UserSpecificServices = "UserSpecificServices";

    public static void SetRawAuthToken(this HttpContext context, string data)
    {
        context.Items[RawAuthToken] = data;
    }

    public static void SetUserContext(this HttpContext context, UserContext data)
    {
        context.Items[UserContextKey] = data;
    }

    public static void SetUserSpecificServices(this HttpContext context, UserSpecificServices data)
    {
        context.Items[UserSpecificServices] = data;
    }

    public static string? GetRawAuthToken(this HttpContext context)
    {
        return context.Items.TryGetValue(RawAuthToken, out var value)
            ? value as string
            : null;
    }

    public static UserContext? GetUserContext(this HttpContext context)
    {
        return context.Items.TryGetValue(UserContextKey, out var value)
            ? value as UserContext
            : null;
    }

    public static UserSpecificServices? GetUserSpecificServices(this HttpContext context)
    {
        return context.Items.TryGetValue(UserSpecificServices, out var value)
            ? value as UserSpecificServices
            : null;
    }
}
