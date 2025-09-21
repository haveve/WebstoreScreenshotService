namespace WebsiteScreenshotService;

public static class Constants
{
    public static class Claims
    {
        public const string UserId = "userId";
        public const string ScreenshotId = "screenshotId";
        public const string WebsiteUrl = "websiteUrl";
        public const string TokenType = "tokenType";

        public enum TokenTypes
        {
            None = 0,
            Authorization = 1,
            Refresh = 2,
            Confirmation = 4,
        }
    }
}
