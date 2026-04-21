using WebsiteScreenshotService.Services.Synchronization;
using System.Text.Json;
using Microsoft.Extensions.Caching.Distributed;

namespace WebsiteScreenshotService.Services.Caching;

public class CacheManager : ICacheManager
{
    private readonly IDistributedCache _cache;
    private readonly IAsyncKeyedLocker _locker;

    public CacheManager(
        IDistributedCache cache,
        IAsyncKeyedLocker locker)
    {
        _cache = cache;
        _locker = locker;
    }

    private static string Key(CacheKey key) => key.Build();

    public async Task<T?> GetAsync<T>(CacheKey key)
    {
        var data = await _cache.GetAsync(Key(key));

        return data is null
            ? default
            : JsonSerializer.Deserialize<T>(data);
    }

    public async Task SetAsync<T>(
        CacheKey key,
        T value,
        CacheEntryOptions? options = null)
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

    public async Task<T> GetOrSetAsync<T>(
        CacheKey key,
        Func<Task<T>> factory,
        CacheEntryOptions? options = null)
    {
        var cached = await GetAsync<T>(key);
        if (cached is not null)
            return cached;

        using (await _locker.AcquireAsync(Key(key)))
        {
            cached = await GetAsync<T>(key);
            if (cached is not null)
                return cached;

            var value = await factory();

            if (value is not null)
                await SetAsync(key, value, options);

            return value;
        }
    }
}