using Microsoft.Playwright;

namespace ScreenshotWorker.Services;

public sealed class PooledContext : IAsyncDisposable
{
    private readonly IBrowserContext _context;
    private readonly Action _onDispose;

    internal PooledContext(IBrowserContext context, Action onDispose)
    {
        _context = context;
        _onDispose = onDispose;
    }

    public IBrowserContext Context => _context;

    public async ValueTask DisposeAsync()
    {
        try
        {
            await _context.CloseAsync();
        }
        finally
        {
            _onDispose();
        }
    }
}
