namespace ScreenshotWorker;

public interface IApplicationLifetimeManager: IDisposable
{
    public CancellationToken CancellationToken { get; }

    public Task StartApplicationAsync();

    public Task StopApplicationAsync();
}
