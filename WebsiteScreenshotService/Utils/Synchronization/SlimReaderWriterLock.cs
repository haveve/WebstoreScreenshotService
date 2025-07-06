using Nito.AsyncEx;

namespace WebsiteScreenshotService.Utils.Synchronization;

public class AsyncSlimReaderWriterLock
{
    private readonly AsyncReaderWriterLock _semaphore = new();

    public Task<IDisposable> EnterReadLockAsync()
        => _semaphore.ReaderLockAsync();

    public Task<IDisposable> EnterWriteLockAsync()
        => _semaphore.WriterLockAsync();
}
