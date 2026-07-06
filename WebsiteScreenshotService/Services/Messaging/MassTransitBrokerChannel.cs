
using MassTransit;
namespace WebsiteScreenshotService.Services.Messaging;

public class MassTransitBrokerChannel(ISendEndpointProvider sendEndpointProvider) : IBrokerChannel
{
    private readonly ISendEndpointProvider _sendEndpointProvider = sendEndpointProvider;

    public async Task PublishAsync<T>(
        T message,
        string routeKey,
        CancellationToken cancellationToken = default) where T : class
    {
        var endpoint = await _sendEndpointProvider.GetSendEndpoint(
            new Uri($"queue:{routeKey}")
        );

        await endpoint.Send(message, cancellationToken);
    }
}