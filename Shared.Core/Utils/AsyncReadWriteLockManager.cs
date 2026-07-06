using System.Collections.Concurrent;
using Shared.Core.Utils.Synchronization;

namespace Shared.Core.Utils;

public sealed class AsyncReadWriteLockManager<TKey> where TKey : notnull
{
    private readonly ConcurrentDictionary<TKey, LockWrapper> _locks = [];

    public async Task<IDisposable> EnterReadAsync(TKey key)
    {
        var wrapper = _locks.GetOrAdd(key, _ => new LockWrapper());
        var handle = await wrapper.Lock.EnterReadLockAsync();

        return new Releaser(key, this, handle, wrapper);
    }

    public async Task<IDisposable> EnterWriteAsync(TKey key)
    {
        var wrapper = _locks.GetOrAdd(key, _ => new LockWrapper());
        var handle = await wrapper.Lock.EnterWriteLockAsync();

        return new Releaser(key, this, handle, wrapper);
    }

    private void Release(TKey key, LockWrapper wrapper)
    {
        if (wrapper.DecrementUsage() == 0)
            _locks.TryRemove(key, out _);
    }

    private sealed class LockWrapper
    {
        public AsyncSlimReaderWriterLock Lock { get; } = new();
        private int _usage = 0;

        public IDisposable IncrementUsage()
        {
            Interlocked.Increment(ref _usage);
            return new UsageTracker(this);
        }

        public int DecrementUsage() => Interlocked.Decrement(ref _usage);

        private sealed class UsageTracker(LockWrapper owner) : IDisposable
        {
            private LockWrapper? _owner = owner;

            public void Dispose()
            {
                _owner?.DecrementUsage();
                _owner = null;
            }
        }
    }

    private sealed class Releaser : IDisposable
    {
        private readonly TKey _key;
        private readonly AsyncReadWriteLockManager<TKey> _manager;
        private readonly IDisposable _innerLock;
        private readonly LockWrapper _wrapper;
        private int _disposed;

        public Releaser(TKey key, AsyncReadWriteLockManager<TKey> manager, IDisposable innerLock, LockWrapper wrapper)
        {
            _key = key;
            _manager = manager;
            _innerLock = innerLock;
            _wrapper = wrapper;
            _wrapper.IncrementUsage();
        }

        public void Dispose()
        {
            //if wasn't disposed yet, release the lock
            if (Interlocked.Exchange(ref _disposed, 1) == 0)
            {
                _innerLock.Dispose();
                _manager.Release(_key, _wrapper);
            }
        }
    }
}
