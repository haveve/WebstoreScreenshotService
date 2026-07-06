namespace WebsiteScreenshotService.Services.Caching.Services;

public interface IUserCacheService
{
    CacheKey Profile(Guid userId);
    
    Task InvalidateAsync();

    Task InvalidateUserAsync(Guid userId);
}
