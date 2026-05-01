
using MassTransit;
namespace WebsiteScreenshotService.Services.Messaging;

public class MassTransitBrokerChannel(IPublishEndpoint publishEndpoint) : IBrokerChannel
{
    private readonly IPublishEndpoint _publishEndpoint = publishEndpoint;

    public async Task PublishAsync<T>(
        T message,
        string routeKey,
        CancellationToken cancellationToken = default) where T : class
    {
        await _publishEndpoint.Publish(message, context =>
        {
            context.Headers.Set("routeKey", routeKey);
        }, cancellationToken);
    }
}