using Microsoft.Extensions.Caching.Distributed;
using System.Text.Json;
using WebsiteScreenshotService.Services.Synchronization;
using WebsiteScreenshotService.Utils;

namespace WebsiteScreenshotService.Services.Caching;

public class CacheManager(IDistributedCache cache) : ICacheManager
{
    private readonly IDistributedCache _cache = cache;
    private readonly AsyncKeyedLocker<string> _locker = new();

    private static string Key(CacheKey key) => key.Build();

    public async Task<T?> GetAsync<T>(CacheKey key) where T : class
    {
        var data = await _cache.GetAsync(Key(key));

        return data is null
            ? default
            : JsonSerializer.Deserialize<T>(data);
    }

    public async Task SetAsync<T>(
        CacheKey key,
        T value,
        CacheEntryOptions? options = null) where T : class
    {
        var entry = new DistributedCacheEntryOptions();

        if (options?.AbsoluteExpirationRelativeToNow is not null)
            entry.AbsoluteExpirationRelativeToNow = options.AbsoluteExpirationRelativeToNow;

        if (options?.SlidingExpiration is not null)
            entry.SlidingExpiration = options.SlidingExpiration;

        var bytes = JsonSerializer.SerializeToUtf8Bytes(value);

        await _cache.SetAsync(Key(key), bytes, entry);
    }

    public async Task RemoveAsync(CacheKey key)
        => await _cache.RemoveAsync(Key(key));

    public async Task<Result<T>> GetOrSetAsync<T>(
        CacheKey key,
        Func<Task<Result<T>>> factory,
        CacheEntryOptions? options = null) where T : class
    {
        var cached = await GetAsync<T>(key);
        if (cached is not null)
            return Result<T>.Success(cached);

        using (await _locker.AcquireAsync(Key(key)))
        {
            cached = await GetAsync<T>(key);
            if (cached is not null)
                return Result<T>.Success(cached);

            var value = await factory();

            if (value.IsSuccess)
                await SetAsync(key, value, options);

            return value;
        }
    }
}