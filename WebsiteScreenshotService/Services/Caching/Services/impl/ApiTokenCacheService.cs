namespace WebsiteScreenshotService.Services.Caching.Services.impl;

public class ApiTokenCacheService(ICacheGroupStateStore state): CacheGroupService(state), IApiTokenCacheService
{
    protected override string GroupName => "api-token";

    public CacheKey ByHash(string tokenHash)
        => new SimpleKey($"{GlobalPrefix}:hash:{tokenHash}");

    public CacheKey ActiveList(Guid userId)
        => new SimpleKey($"{UserPrefix(userId)}:list");

    public Task InvalidateAsync()
        => BumpGlobalVersionAsync();

    public Task InvalidateUserAsync(Guid userId)
        => BumpUserVersionAsync(userId);
}