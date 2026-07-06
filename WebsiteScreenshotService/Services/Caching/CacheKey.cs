namespace WebsiteScreenshotService.Services.Caching;

public abstract record CacheKey
{
    public abstract string Build();
}
