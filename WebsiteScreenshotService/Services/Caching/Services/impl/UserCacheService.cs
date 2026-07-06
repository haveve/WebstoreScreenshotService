namespace WebsiteScreenshotService.Services.Caching.Services.impl;

public class UserCacheService(ICacheGroupStateStore state) : CacheGroupService(state), IUserCacheService
{
    protected override string GroupName => "user";

    public CacheKey Profile(Guid userId)
        => new SimpleKey($"{UserPrefix(userId)}:profile");

    public Task InvalidateAsync()
        => BumpGlobalVersionAsync();

    public Task InvalidateUserAsync(Guid userId)
        => BumpUserVersionAsync(userId);
}
