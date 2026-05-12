namespace WebsiteScreenshotService.Services.Caching.Services;

public interface IApiTokenCacheService
{
    CacheKey ByHash(string tokenHash);

    CacheKey ActiveList(Guid userId);

    Task InvalidateAsync();

    Task InvalidateUserAsync(Guid userId);
}