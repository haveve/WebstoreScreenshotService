using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using RabbitMQ.Client;
using RabbitMQ.Client.Events;
using ScreenshotWorker.Exceptions;
using ScreenshotWorker.Repositories;
using ScreenshotWorker.Serialization;
using ScreenshotWorker.Services;
using ScreenshotWorker.Settings;
using Shared.Core.Contracts.ScreeshotModel;
using Shared.Core.Contracts.ScreeshotModel.Validation;
using System.ComponentModel.DataAnnotations;
using System.Text;
using System.Text.Json;

namespace ScreenshotWorker.Managers;

public class MessageBrokerManager(ILogger<MessageBrokerManager> logger, IBrowserService browserService, IScreenshotRepository screenshotRepository, IScreenshotService screenshotServiceCommunicator, IOptions<MessageBrokerSettings> configuration, MakeScreenshotModelValidator makeScreenshotModelValidator) : IMessageBrokerManager
{
    private readonly ILogger<MessageBrokerManager> _logger = logger;
    private readonly IBrowserService _browserService = browserService;
    private readonly MessageBrokerSettings _configuration = configuration.Value;
    private readonly IScreenshotRepository _screenshotRepository = screenshotRepository;
    private readonly IScreenshotService _screenshotServiceCommunicator = screenshotServiceCommunicator;
    private readonly MakeScreenshotModelValidator _makeScreenshotModelValidator = makeScreenshotModelValidator;

    private static readonly string[] valueCannotBeParsedErrors = ["_root: object wasn't parsed correctly"];

    class RabbitMqRequest
    {
        [Required]
        public MakeScreenshotModel Message { get; set; } = null!;
    }

    public async Task InitializeAsync()
    {
        var channel = await ConfigureAsync();

        var consumer = new AsyncEventingBasicConsumer(channel);

        consumer.ReceivedAsync += async (_, ea) =>
        {
            string? confirmationToken = null;
            try
            {
                var parsedValue = CustomJsonSerializer.Deserialize<RabbitMqRequest>(ea.Body.Span)?.Message;

                var validationResult = parsedValue is not null
                    ? _makeScreenshotModelValidator.Validate(parsedValue)
                    : null;

                if (parsedValue is null || validationResult is null || !validationResult.IsValid)
                {
                    var errors = validationResult?.Errors?.Select(e => $"{e.Path}: {e.Message}")
                        ?? valueCannotBeParsedErrors;
                    await NackAsync(channel, ea.DeliveryTag, parsedValue, errors);
                    return;
                }

                confirmationToken = parsedValue.ConfirmationToken;

                var screenshotData = await _browserService.MakeScreenshotAsync(parsedValue.ScreenshotOptionsModel);

                var saveScreenshotModel = new SaveScreenshotModel(parsedValue.ScreenshotId,
                     parsedValue.UserInformation.UserId.ToString(),
                     screenshotData,
                     parsedValue.ScreenshotOptionsModel.ScreenshotType);

                var savedSuccessfully = await _screenshotRepository.SaveScreenshot(saveScreenshotModel);

                await _screenshotServiceCommunicator.ConfirmScreenshotAttemptAsync(parsedValue.ConfirmationToken);
                await channel.BasicAckAsync(ea.DeliveryTag, multiple: false);

            }
            catch (JsonException ex)
            {
                var messageJsonAsString = Encoding.UTF8.GetString(ea.Body.ToArray());
                var errors = new string[] { ex.Message };

                await NackAsync(channel, ea.DeliveryTag, messageJsonAsString, errors);
            }
            catch(ProcessAbortException ex)
            {
                _logger.LogError(ex, "An error occurred while processing the screenshot: {Message}", Encoding.UTF8.GetString(ea.Body.ToArray()));

                await channel.BasicNackAsync(ea.DeliveryTag, multiple: false, requeue: false);
                if (confirmationToken is not null)
                    await _screenshotServiceCommunicator.FailedScreenshotAttemptAsync(confirmationToken);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "A redeemable error occurred while processing the screenshot: {Message}", Encoding.UTF8.GetString(ea.Body.ToArray()));

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

        var connection = await factory.CreateConnectionAsync();
        var channel = await connection.CreateChannelAsync();

        await channel.QueueDeclarePassiveAsync(queue: _configuration.Queue.Name);

        await channel.BasicQosAsync(prefetchCount: _configuration.Connection.PrefetchCount, prefetchSize: 0, global: false);

        return channel;
    }

    private ValueTask NackAsync(IChannel channel, ulong deliveryTag, object? message, IEnumerable<string> errors)
    {
        _logger.LogError("Received request has invalid format message: {Message}, errors: {Errors}", message, errors);
        return channel.BasicNackAsync(deliveryTag, multiple: false, requeue: false);
    }

    public ValueTask DisposeAsync()
    {
        return ValueTask.CompletedTask;
    }
}
