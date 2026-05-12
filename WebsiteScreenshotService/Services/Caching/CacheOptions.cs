namespace WebsiteScreenshotService.Services.Caching;

public static class CacheOptions
{
    public static class Category
    {
        public static CacheEntryOptions List { get; } = CacheEntryOptions.Absolute(TimeSpan.FromMinutes(10));

        public static CacheEntryOptions Entry { get; } = CacheEntryOptions.Absolute(TimeSpan.FromMinutes(10));   
    }

    public static class User
    {
        public static CacheEntryOptions List { get; } = CacheEntryOptions.Absolute(TimeSpan.FromMinutes(10));

        public static CacheEntryOptions Entry { get; } = CacheEntryOptions.Absolute(TimeSpan.FromMinutes(10));
    }

    public static class Screenshot
    {
        public static CacheEntryOptions List { get; } = CacheEntryOptions.Absolute(TimeSpan.FromMinutes(10));

        public static CacheEntryOptions Entry { get; } = CacheEntryOptions.Absolute(TimeSpan.FromMinutes(10));
    }

    public static class Token
    {
        public static CacheEntryOptions List { get; } = CacheEntryOptions.Absolute(TimeSpan.FromMinutes(10));

        public static CacheEntryOptions Entry { get; } = CacheEntryOptions.Absolute(TimeSpan.FromMinutes(10));
    }
}
