namespace ScreenshotWorker.Managers;

public interface IApplicationLifetimeManager : IDisposable
{
    public CancellationToken CancellationToken { get; }

    public Task SetupApplicationAsync();

    public Task StartApplicationAsync();

    public Task StopApplicationAsync();
}
