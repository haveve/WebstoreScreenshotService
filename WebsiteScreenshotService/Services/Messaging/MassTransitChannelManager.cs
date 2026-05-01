using MassTransit;

namespace WebsiteScreenshotService.Services.Messaging;

public class MassTransitChannelManager(IPublishEndpoint publishEndpoint) : IMessageBrokerChannelManager
{
    private readonly IPublishEndpoint _publishEndpoint = publishEndpoint;

    public Task<IBrokerChannel> GetChannelAsync(CancellationToken cancellationToken = default)
    {
        IBrokerChannel channel = new MassTransitBrokerChannel(_publishEndpoint);
        return Task.FromResult(channel);
    }
}