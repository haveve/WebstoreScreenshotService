namespace WebsiteScreenshotService.Services.Caching.Services.impl;

public class ScreenshotCacheService(ICacheGroupStateStore state) : CacheGroupService(state), IScreenshotCacheService
{
    protected override string GroupName => "screenshot";

    public CacheKey ById(string screenshotId)
        => new SimpleKey($"{GlobalPrefix}:id:{screenshotId}");

    public CacheKey List(Guid userId, PagingCacheModel? paging)
        => new SimpleKey($"{UserPrefix(userId)}:list{FormatPaging(paging)}");

    public Task InvalidateAsync()
        => BumpGlobalVersionAsync();

    public Task InvalidateUserAsync(Guid userId)
        => BumpUserVersionAsync(userId);

    private static string FormatPaging(PagingCacheModel? paging)
    {
        if (paging is null)
            return ":p1";
        
        return $":p{paging.Page}:s:{paging.PageSize}";
    }
}