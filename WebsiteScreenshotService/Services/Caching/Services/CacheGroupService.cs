namespace WebsiteScreenshotService.Services.Caching.Services;

public abstract class CacheGroupService(ICacheGroupStateStore state)
{
    private readonly ICacheGroupStateStore _state = state;

    protected abstract string GroupName { get; }

    protected int GlobalVersion
        => _state.GetVersion(GroupName);

    protected int UserVersion(Guid userId)
        => _state.GetVersion($"{GroupName}:user:{userId}");

    protected string GlobalPrefix
        => $"{GroupName}:v{GlobalVersion}";

    protected string UserPrefix(Guid userId)
        => $"{GlobalPrefix}:user:{userId}:v{UserVersion(userId)}";

    protected Task BumpGlobalVersionAsync()
        => _state.BumpVersionAsync(GroupName);

    protected Task BumpUserVersionAsync(Guid userId)
        => _state.BumpVersionAsync($"{GroupName}:user:{userId}");
}
