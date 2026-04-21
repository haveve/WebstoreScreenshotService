using System.Collections.Concurrent;
using System.Threading;

namespace WebsiteScreenshotService.Services.Synchronization;

public class AsyncKeyedLocker : IAsyncKeyedLocker
{
    private readonly ConcurrentDictionary<string, SemaphoreSlim> _locks = new();

    public async Task<IDisposable> AcquireAsync(
        string key,
        CancellationToken cancellationToken = default)
    {
        var semaphore = _locks.GetOrAdd(key, _ => new SemaphoreSlim(1, 1));

        await semaphore.WaitAsync(cancellationToken);

        return new Releaser(key, semaphore, _locks);
    }

    private sealed class Releaser : IDisposable
    {
        private readonly string _key;
        private readonly SemaphoreSlim _semaphore;
        private readonly ConcurrentDictionary<string, SemaphoreSlim> _locks;
        private bool _disposed;

        public Releaser(
            string key,
            SemaphoreSlim semaphore,
            ConcurrentDictionary<string, SemaphoreSlim> locks)
        {
            _key = key;
            _semaphore = semaphore;
            _locks = locks;
        }

        public void Dispose()
        {
            if (_disposed)
                return;

            _semaphore.Release();

            // 🔥 Cleanup: remove unused locks
            if (_semaphore.CurrentCount == 1)
            {
                _locks.TryRemove(_key, out _);
            }

            _disposed = true;
        }
    }
}
