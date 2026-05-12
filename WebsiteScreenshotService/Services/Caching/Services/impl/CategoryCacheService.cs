namespace WebsiteScreenshotService.Services.Caching.Services.impl;

public class CategoryCacheService(ICacheGroupStateStore state) : CacheGroupService(state), ICategoryCacheService
{
    protected override string GroupName => "category";

    public CacheKey List(Guid userId)
        => new SimpleKey($"{UserPrefix(userId)}:list");

    public CacheKey ById(Guid categoryId)
        => new SimpleKey($"{GlobalPrefix}:id:{categoryId}");

    public Task InvalidateAsync()
        => BumpGlobalVersionAsync();

    public Task InvalidateUserAsync(Guid userId)
        => BumpUserVersionAsync(userId);
}