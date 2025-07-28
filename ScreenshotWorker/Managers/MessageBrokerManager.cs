using RabbitMQ.Client.Events;
using RabbitMQ.Client;
using ScreenshotWorker.Serialization;
using ScreenshotWorker.Model;
using System.Text.Json;
using Microsoft.Extensions.Logging;
using System.Text;
using ScreenshotWorker.Services;
using Microsoft.Extensions.Options;
using ScreenshotWorker.Settings;
using ScreenshotWorker.Repositories;

namespace ScreenshotWorker.Managers;

public class MessageBrokerManager(ILogger<MessageBrokerManager> logger, IBrowserService browserService, IScreenshotRepository screenshotRepository, IScreenshotService screenshotServiceCommunicator, IOptions<MessageBrokerSettings> configuration) : IMessageBrokerManager
{
    private readonly ILogger<MessageBrokerManager> _logger = logger;
    private readonly IBrowserService _browserService = browserService;
    private readonly MessageBrokerSettings _configuration = configuration.Value;
    private readonly IScreenshotRepository _screenshotRepository = screenshotRepository;
    private readonly IScreenshotService _screenshotServiceCommunicator = screenshotServiceCommunicator;

    public async Task InitializeAsync()
    {
        var channel = await ConfigureAsync();

        var consumer = new AsyncEventingBasicConsumer(channel);

        string? confirmationToken = null;

        consumer.ReceivedAsync += async (_, ea) =>
        {
            try
            {
                var (errors, parsedValue) = CustomJsonSerializer.TryDeserialize<MakeScreenshotModel>(ea.Body.Span);

                if (parsedValue is null)
                {
                    await NackAsync(channel, ea.DeliveryTag, parsedValue, errors);
                    return;
                }

                confirmationToken = parsedValue.ConfirmationToken;

                await channel.BasicAckAsync(ea.DeliveryTag, multiple: false);

                var screenshotData = await _browserService.MakeScreenshotAsync(parsedValue.ScreenshotOptionsModel);

                _ = Task.Run(async () =>
                {
                    try
                    {
                        var savedSuccessfully = await _screenshotRepository.SaveScreenshot(
                             parsedValue.ScreenshotId,
                             parsedValue.UserInformation.UserId.ToString(),
                             screenshotData,
                             parsedValue.ScreenshotOptionsModel.ScreenshotType);
                    }
                    catch
                    {
                        await _screenshotServiceCommunicator.RedeemScreenshotAttemptAsync(parsedValue.ConfirmationToken);
                    }
                });

            }
            catch (JsonException ex)
            {
                var messageJsonAsString = Encoding.UTF8.GetString(ea.Body.ToArray());
                var errors = new string[] { ex.Message };

                await NackAsync(channel, ea.DeliveryTag, messageJsonAsString, errors);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "An error occurred while processing the message: {Message}", Encoding.UTF8.GetString(ea.Body.ToArray()));

                await channel.BasicNackAsync(ea.DeliveryTag, multiple: false, requeue: false);
                if (confirmationToken is not null)
                    await _screenshotServiceCommunicator.RedeemScreenshotAttemptAsync(confirmationToken);
            }
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
