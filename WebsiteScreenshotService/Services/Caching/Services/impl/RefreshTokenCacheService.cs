namespace WebsiteScreenshotService.Services.Caching.Services.impl;

public class RefreshTokenCacheService(ICacheGroupStateStore state) : CacheGroupService(state), IRefreshTokenCacheService
{
    protected override string GroupName => "refresh-token";

    public CacheKey ByHash(string tokenHash)
        => new SimpleKey($"{GlobalPrefix}:hash:{tokenHash}");

    public CacheKey ActiveList(Guid userId)
        => new SimpleKey($"{UserPrefix(userId)}:list");

    public Task InvalidateAsync()
        => BumpGlobalVersionAsync();

    public Task InvalidateUserAsync(Guid userId)
        => BumpUserVersionAsync(userId);
}