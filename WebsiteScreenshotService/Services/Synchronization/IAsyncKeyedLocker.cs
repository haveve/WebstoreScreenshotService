namespace WebsiteScreenshotService.Services.Synchronization;

public interface IAsyncKeyedLocker
{
    Task<IDisposable> AcquireAsync(string key, CancellationToken cancellationToken = default);
}
