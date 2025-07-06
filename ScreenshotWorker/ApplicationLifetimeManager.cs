namespace ScreenshotWorker;

public sealed class ApplicationLifetimeManager(IMessageBrokerManager messageBrokerManager) : IApplicationLifetimeManager
{
    private readonly IMessageBrokerManager _messageBrokerManager = messageBrokerManager;
    private readonly CancellationTokenSource _cts = new();
    private bool _started = false;
    private bool _disposed = false;

    public CancellationToken CancellationToken => _cts.Token;

    public async Task StartApplicationAsync()
    {
        ThrowIfDisposed();

        if (_started)
            return;

        await _messageBrokerManager.InitializeAsync();

        _started = true;
    }

    public Task StopApplicationAsync()
    {
        ThrowIfDisposed();

        if (_started)
        {
            _cts.Cancel();
            Dispose();
            _started = false;
        }

        return Task.CompletedTask;
    }

    private void ThrowIfDisposed()
    {
        if (_disposed)
            throw new ObjectDisposedException(nameof(ApplicationLifetimeManager), "The application lifetime manager has already been disposed.");
    }

    public void Dispose()
    {
        if (_disposed)
            return;

        _disposed = true;
        _cts.Dispose();
    }
}