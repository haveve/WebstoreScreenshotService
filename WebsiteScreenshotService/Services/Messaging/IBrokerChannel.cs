namespace WebsiteScreenshotService.Services.Messaging;

public interface IBrokerChannel
{
    public Task PublishAsync<T>(T message, string routeKey, CancellationToken cancellationToken = default);
}
