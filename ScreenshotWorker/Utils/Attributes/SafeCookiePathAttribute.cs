namespace ScreenshotWorker.Utils.Attributes;

/// <summary>
/// Validates cookie path strictly (RFC 6265).
/// </summary>
public class SafeCookiePathAttribute : SafeCookieStringAttribute
{
    private static readonly string PathPattern =
        @"^\/[a-zA-Z0-9\/_\-\.]*$"; // path starting with /, valid chars

    public SafeCookiePathAttribute(int maxLength)
        : base(maxLength, PathPattern)
    {
    }
}