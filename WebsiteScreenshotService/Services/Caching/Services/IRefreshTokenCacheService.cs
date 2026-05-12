namespace WebsiteScreenshotService.Services.Caching.Services;

public interface IRefreshTokenCacheService
{
    CacheKey ByHash(string tokenHash);

    CacheKey ActiveList(Guid userId);

    Task InvalidateUserAsync(Guid userId);

    Task InvalidateAsync();
}
