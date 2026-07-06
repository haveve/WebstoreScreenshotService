using System.Collections.Concurrent;

namespace WebsiteScreenshotService.Services.Caching.Services;

public class InMemoryCacheGroupStateStore: ICacheGroupStateStore
{
    private readonly ConcurrentDictionary<string, int> _versions = [];

    public int GetVersion(string groupName)
        => _versions.GetOrAdd(groupName, 1);

    public Task<int> GetVersionAsync(string groupName)
        => Task.FromResult(GetVersion(groupName));

    public Task BumpVersionAsync(string groupName)
    {
        _versions.AddOrUpdate(groupName, 2, (_, v) => v + 1);
        return Task.CompletedTask;
    }
}
