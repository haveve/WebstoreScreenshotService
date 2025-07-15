using RabbitMQ.Client;
using System.Text.Json;

namespace WebsiteScreenshotService.Services.Messaging;

public class RabbitMqBrokerChannel(IChannel channel, string exchangeName) : IBrokerChannel
{
    private readonly IChannel _channel = channel;
    private readonly string _exchangeName = exchangeName;

    public async Task PublishAsync<T>(T message, string routeKey, CancellationToken cancellationToken = default)
    {
        var props = new BasicProperties
        {
            ContentType = "application/json",
            DeliveryMode = DeliveryModes.Persistent,
        };

        var body = JsonSerializer.SerializeToUtf8Bytes(message);
        await _channel.BasicPublishAsync(_exchangeName, routeKey, mandatory: true, props, body, cancellationToken);
    }
}