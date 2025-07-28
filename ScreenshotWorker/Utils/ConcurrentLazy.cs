namespace ScreenshotWorker.Utils;

/// <summary>
/// Provides a mechanism for lazy asynchronous initialization of a value.
/// </summary>
/// <typeparam name="T">The type of the value to be initialized.</typeparam>
public class ConcurrentLazy<T>(Func<T> factory)
{
    private readonly Func<T> _factory = factory;

    private readonly SemaphoreSlim _semaphore = new(initialCount: 1, maxCount: 1);

    private T? _value = default;

    /// <summary>
    /// Gets the value asynchronously, initializing it if necessary.
    /// </summary>
    /// <returns>A task that represents the asynchronous operation. The task result contains the initialized value.</returns>
    public T GetValue()
    {
        if (_value is not null && !_value.Equals(default(T)))
            return _value;

        try
        {
            _semaphore.Wait();

            if (_value is null || _value.Equals(default(T)))
                _value = _factory();

            return _value;
        }
        finally
        {
            _semaphore.Release();
        }
    }
}

