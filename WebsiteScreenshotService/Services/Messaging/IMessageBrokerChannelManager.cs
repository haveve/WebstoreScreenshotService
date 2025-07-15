namespace WebsiteScreenshotService.Services.Messaging;

public interface IMessageBrokerChannelManager
{
    Task<IBrokerChannel> GetChannelAsync(CancellationToken cancellationToken = default);
}