namespace WebsiteScreenshotService.Services.Caching;

public interface ICacheManager
{
    Task<T?> GetAsync<T>(CacheKey key);

    Task SetAsync<T>(
        CacheKey key,
        T value,
        CacheEntryOptions? options = null);

    Task RemoveAsync(CacheKey key);

    Task<T> GetOrSetAsync<T>(
        CacheKey key,
        Func<Task<T>> factory,
        CacheEntryOptions? options = null);
}

public sealed class CacheEntryOptions
{
    /// <summary>
    /// Fixed TTL from now (e.g. cache for 10 minutes)
    /// </summary>
    public TimeSpan? AbsoluteExpirationRelativeToNow { get; init; }

    /// <summary>
    /// Sliding expiration (resets on access)
    /// </summary>
    public TimeSpan? SlidingExpiration { get; init; }

    public static CacheEntryOptions Absolute(TimeSpan ttl)
        => new() { AbsoluteExpirationRelativeToNow = ttl };

    public static CacheEntryOptions Sliding(TimeSpan ttl)
        => new() { SlidingExpiration = ttl };
}