namespace WebsiteScreenshotService.Services.Messaging;

public interface IMessageBrokerManager
{
    Task<bool> SendMessageAsync<T>(T message, string routeKey, CancellationToken cancellationToken = default);
}
