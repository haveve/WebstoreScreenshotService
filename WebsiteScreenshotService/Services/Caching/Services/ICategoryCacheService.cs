namespace WebsiteScreenshotService.Services.Caching.Services;

public interface ICategoryCacheService
{
    CacheKey List(Guid userId);
    
    CacheKey ById(Guid categoryId);
    
    Task InvalidateAsync();

    Task InvalidateUserAsync(Guid userId);
}
