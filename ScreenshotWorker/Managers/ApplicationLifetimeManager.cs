using ScreenshotWorker.Services;

namespace ScreenshotWorker.Managers;

public sealed class ApplicationLifetimeManager(IMessageBrokerManager messageBrokerManager, BrowserPool browserPool) : IApplicationLifetimeManager
{
    private readonly IMessageBrokerManager _messageBrokerManager = messageBrokerManager;
    private readonly BrowserPool _browserPool = browserPool;
    private readonly CancellationTokenSource _cts = new();
    private readonly SemaphoreSlim _applicationLifetimeSemaphore = new(1, 1);
    private bool _started = false;
    private bool _disposed = false;

    public CancellationToken CancellationToken => _cts.Token;

    public async Task SetupApplicationAsync()
    {
        ThrowIfDisposed();
        if (_started)
            return;
        try
        {
            await _applicationLifetimeSemaphore.WaitAsync();

            ThrowIfDisposed();
            if (_started)
                return;

            var exitCode = Microsoft.Playwright.Program.Main(["install", "chromium"]);
            //"--with-deps"

            if (exitCode != 0)
                throw new Exception($"Playwright exited with code {exitCode}");
        }
        finally
        {
            _applicationLifetimeSemaphore.Release();
        }
    }

    public async Task StartApplicationAsync()
    {
        ThrowIfDisposed();

        if (_started)
            return;

        try
        {
            await _applicationLifetimeSemaphore.WaitAsync();

            ThrowIfDisposed();

            if (_started)
                return;

            //EnsureBrowserInstalled();
            await _browserPool.InitializeAsync();
            await _messageBrokerManager.InitializeAsync();
        }
        finally
        {
            _applicationLifetimeSemaphore.Release();
        }

        _started = true;
    }

    public async Task StopApplicationAsync()
    {
        ThrowIfDisposed();

        if (!_started)
            return;

        try
        {
            await _applicationLifetimeSemaphore.WaitAsync();

            ThrowIfDisposed();

            if (!_started)
                return;

            await _browserPool.DisposeAsync();
            await _messageBrokerManager.DisposeAsync();
            _cts.Cancel();
            Dispose();
            _started = false;
        }
        finally
        {
            _applicationLifetimeSemaphore.Release();
        }

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

    private static bool IsChromiumInstalled(out string baseDir)
    {
        baseDir = OperatingSystem.IsWindows()
            ? Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.LocalApplicationData), "ms-playwright")
            : Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.UserProfile), ".cache", "ms-playwright");

        if (!Directory.Exists(baseDir))
            return false;

        return Directory.EnumerateDirectories(baseDir, "chromium-*", SearchOption.TopDirectoryOnly)
            .SelectMany(dir => Directory.EnumerateFiles(dir, "*", SearchOption.AllDirectories))
            .Any(f => f.EndsWith("chrome") || f.EndsWith("chrome.exe"));
    }

    private static void EnsureBrowserInstalled()
    {
        if (IsChromiumInstalled(out string baseDir))
            return;

        throw new Exception($"Browser wasn't found under path {baseDir}, please check your scripts and start app again");
    }
}