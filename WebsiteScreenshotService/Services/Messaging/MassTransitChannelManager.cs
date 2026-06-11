using MassTransit;

namespace WebsiteScreenshotService.Services.Messaging;

public class MassTransitChannelManager(ISendEndpointProvider publishEndpoint) : IMessageBrokerChannelManager
{
    private readonly ISendEndpointProvider _publishEndpoint = publishEndpoint;

    public Task<IBrokerChannel> GetChannelAsync(CancellationToken cancellationToken = default)
    {
        IBrokerChannel channel = new MassTransitBrokerChannel(_publishEndpoint);
        return Task.FromResult(channel);
    }
}