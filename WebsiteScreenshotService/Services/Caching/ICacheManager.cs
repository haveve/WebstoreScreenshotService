using WebsiteScreenshotService.Utils;

namespace WebsiteScreenshotService.Services.Caching;

public interface ICacheManager
{
    Task<T?> GetAsync<T>(CacheKey key) where T : class;

    Task SetAsync<T>(
        CacheKey key,
        T value,
        CacheEntryOptions? options = null) where T : class;

    Task RemoveAsync(CacheKey key);

    Task<Result<T>> GetOrSetAsync<T>(
        CacheKey key,
        Func<Task<Result<T>>> factory,
        CacheEntryOptions? options = null) where T : class;
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