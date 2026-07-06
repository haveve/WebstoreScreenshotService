using System.Collections.Concurrent;

namespace WebsiteScreenshotService.Services.Synchronization;

public class AsyncKeyedLocker<TKey> where TKey : notnull
{
    private readonly ConcurrentDictionary<TKey, SemaphoreSlim> _locks = new();

    public async Task<IDisposable> AcquireAsync(
        TKey key,
        CancellationToken cancellationToken = default)
    {
        var semaphore = _locks.GetOrAdd(key, _ => new SemaphoreSlim(1, 1));

        await semaphore.WaitAsync(cancellationToken);

        return new Releaser(key, semaphore, _locks);
    }

    private sealed class Releaser : IDisposable
    {
        private readonly TKey _key;
        private readonly SemaphoreSlim _semaphore;
        private readonly ConcurrentDictionary<TKey, SemaphoreSlim> _locks;
        private bool _disposed;

        public Releaser(
            TKey key,
            SemaphoreSlim semaphore,
            ConcurrentDictionary<TKey, SemaphoreSlim> locks)
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

            if (_semaphore.CurrentCount == 1)
            {
                _locks.TryRemove(_key, out _);
            }

            _disposed = true;
        }
    }
}
