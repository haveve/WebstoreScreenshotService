namespace WebsiteScreenshotService.Utils;

/// <summary>
/// Provides a mechanism for lazy asynchronous initialization of a value.
/// </summary>
/// <typeparam name="T">The type of the value to be initialized.</typeparam>
public class LazyAsync<T>(Func<Task<T>> Factory)
{
    private readonly Func<Task<T>> _factory = Factory;

    private readonly SemaphoreSlim _semaphore = new(initialCount: 1, maxCount: 1);

    private T? _value = default;

    /// <summary>
    /// Gets the value asynchronously, initializing it if necessary.
    /// </summary>
    /// <returns>A task that represents the asynchronous operation. The task result contains the initialized value.</returns>
    public async ValueTask<T> GetValueAsync()
    {
        if (_value is not null && !_value.Equals(default(T)))
            return _value;

        try
        {
            await _semaphore.WaitAsync();

            if (_value is null || _value.Equals(default(T)))
                _value = await _factory();

            return _value;
        }
        finally
        {
            _semaphore.Release();
        }
    }
}

