namespace WebsiteScreenshotService.Services.Caching.Services;

public interface ICacheGroupStateStore
{
    int GetVersion(string groupName);
    
    Task<int> GetVersionAsync(string groupName);
    
    Task BumpVersionAsync(string groupName);
}
