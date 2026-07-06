namespace WebsiteScreenshotService.Services.Caching;

public sealed record SimpleKey(string Value) : CacheKey
{
    public override string Build() => Value;
}
