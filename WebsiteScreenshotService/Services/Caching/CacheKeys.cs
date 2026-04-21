namespace WebsiteScreenshotService.Services.Caching;

public static class CacheKeys
{
    public static UserCacheGroup User { get; } = new();
    public static CategoryKeys Category { get; } = new();

    public sealed class UserCacheGroup : CacheGroup
    {
        public override string Name => "user";

        public CacheKey Profile(int userId)
            => new SimpleKey($"{Prefix}:profile:{userId}");

        public CacheKey List()
            => new SimpleKey($"{Prefix}:list");
    }

    public sealed class CategoryKeys
    {
        public const string Group = "category";

        public CacheKey List(Guid userId)
            => new SimpleKey($"{Group}:list:{userId}");

        public CacheKey ById(Guid categoryId)
            => new SimpleKey($"{Group}:id:{categoryId}");
    }
}

public abstract class CacheGroup
{
    private int _version = 1;

    public abstract string Name { get; }

    public int Version => _version;

    public void BumpVersion()
    {
        Interlocked.Increment(ref _version);
    }

    public string Prefix => $"{Name}:v{Version}";
}