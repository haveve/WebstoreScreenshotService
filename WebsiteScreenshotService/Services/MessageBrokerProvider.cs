using Microsoft.Extensions.Options;
using RabbitMQ.Client;
using System.Text.Json;
using WebsiteScreenshotService.Utils;

namespace WebsiteScreenshotService.Services;

public class MessageBrokerProvider: IMessageBrokerProvider
{
    private readonly string _exchangeName;

    private readonly ConnectionConfig _connectionConfig;

    private readonly LazyAsync<IChannel> _channel;

    private readonly ILogger<MessageBrokerProvider> _logger;

    public MessageBrokerProvider(IOptions<MessageBrokerConfigurations> options, ILogger<MessageBrokerProvider> logger)
    {
        _connectionConfig = options.Value.Connection;
        _exchangeName = options.Value.Exchange.Name;
        _logger = logger;
        _channel = new(ConfigureAsync);
    }

    private async Task<IChannel> ConfigureAsync()
    { 
        var factory = new ConnectionFactory
        {
            HostName = _connectionConfig.HostName,
            Port = _connectionConfig.Port,
            UserName = _connectionConfig.UserName,
            Password = _connectionConfig.Password,
        };

        var channelOptions = new CreateChannelOptions(
            publisherConfirmationsEnabled: true,
            publisherConfirmationTrackingEnabled: true,
            outstandingPublisherConfirmationsRateLimiter: null
        );

        var connection = await factory.CreateConnectionAsync();
        return await connection.CreateChannelAsync(channelOptions);
    }

    public async Task<bool> SendMessageAsync<T>(T message, string routeKey, CancellationToken cancellationToken = default)
    {
        try
        {
            var channel = await _channel.GetValueAsync();

            var props = new BasicProperties
            {
                ContentType = "application/json",
                DeliveryMode = DeliveryModes.Persistent
            };

            var body = JsonSerializer.SerializeToUtf8Bytes(message);

            await channel.BasicPublishAsync(_exchangeName, routeKey, mandatory: true, props, body, cancellationToken);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error sending message");
            return false;
        }

        return true;
    }
}

