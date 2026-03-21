namespace ScreenshotWorker.Managers;

public interface IMessageBrokerManager: IAsyncDisposable
{
    public Task InitializeAsync();
}
