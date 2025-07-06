using RabbitMQ.Client.Events;
using RabbitMQ.Client;
using ScreenshotWorker.Serialization;
using ScreenshotWorker.Model;
using System.Text.Json;
using Microsoft.Extensions.Logging;
using System.Text;
using ScreenshotWorker.Services;
using Microsoft.Extensions.Options;

namespace ScreenshotWorker;

public class MessageBrokerManager(ILogger<MessageBrokerManager> logger, IBrowserService browserService, IOptions<MessageBrokerConfigurations> configuration) : IMessageBrokerManager
{
    private readonly ILogger<MessageBrokerManager> _logger = logger;
    private readonly IBrowserService _browserService = browserService;
    private readonly MessageBrokerConfigurations _configuration = configuration.Value;

    public async Task InitializeAsync()
    {
        var channel = await ConfigureAsync();

        var consumer = new AsyncEventingBasicConsumer(channel);

        consumer.ReceivedAsync += async (_, ea) =>
        {
            try
            {
                var (errors, isValid, parsedValue) = CustomJsonSerializer.TryDeserialize<MakeScreenshotModel>(ea.Body.Span);

                if (!isValid)
                {
                    await NackAsync(channel, ea.DeliveryTag, parsedValue, errors);
                    return;
                }

                var image = await _browserService.MakeScreenshotAsync(parsedValue!.ScreenshotOptionsModel);
            }
            catch (JsonException ex)
            {
                var messageJsonAsString = Encoding.UTF8.GetString(ea.Body.ToArray());
                var errors = new string[] { ex.Message };

                await NackAsync(channel, ea.DeliveryTag, messageJsonAsString, errors);
                return;
            }

            await channel.BasicAckAsync(ea.DeliveryTag, multiple: false);
        };

        await channel.BasicConsumeAsync(_configuration.Queue.Name, autoAck: false, consumer);
    }

    private async Task<IChannel> ConfigureAsync()
    {
        var factory = new ConnectionFactory
        {
            ConsumerDispatchConcurrency = _configuration.Connection.ConsumerDispatchConcurrency,
            HostName = _configuration.Connection.HostName,
            Port = _configuration.Connection.Port,
            Password = _configuration.Connection.Password,
            UserName = _configuration.Connection.UserName,
            VirtualHost = _configuration.Connection.VirtualHost,
        };

        using var connection = await factory.CreateConnectionAsync();
        using var channel = await connection.CreateChannelAsync();

        await channel.QueueDeclarePassiveAsync(queue: _configuration.Queue.Name);

        await channel.BasicQosAsync(prefetchCount: _configuration.Connection.PrefetchCount, prefetchSize: 0, global: false);

        return channel;
    }

    private ValueTask NackAsync(IChannel channel, ulong deliveryTag, object? message, IEnumerable<string> errors)
    {
        _logger.LogError("Received request has invalid format message: {Message}, errors: {Errors}", message, errors);
        return channel.BasicNackAsync(deliveryTag, multiple: false, requeue: false);
    }
}
