namespace WebsiteScreenshotService.Services.Caching.Services;

public interface IScreenshotCacheService
{
    CacheKey ById(string screenshotId);

    CacheKey List(Guid userId, PagingCacheModel? paging);
    
    Task InvalidateAsync();

    Task InvalidateUserAsync(Guid userId);
}

public record PagingCacheModel(int Page, int PageSize);
