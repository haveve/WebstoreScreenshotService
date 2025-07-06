namespace WebsiteScreenshotService.Services;

public interface IMessageBrokerProvider
{
    Task<bool> SendMessageAsync<T>(T message, string routeKey, CancellationToken cancellationToken = default);
}
