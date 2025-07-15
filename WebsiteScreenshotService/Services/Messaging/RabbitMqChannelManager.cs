using Microsoft.Extensions.Options;
using RabbitMQ.Client.Exceptions;
using RabbitMQ.Client;
using WebsiteScreenshotService.Configurations;

namespace WebsiteScreenshotService.Services.Messaging;

public class RabbitMqChannelManager : IMessageBrokerChannelManager, IAsyncDisposable
{
    private readonly ConnectionFactory _factory;
    private readonly CreateChannelOptions _channelOptions;
    private readonly ILogger<RabbitMqChannelManager> _logger;
    private readonly string _exchangeName;
    private readonly SemaphoreSlim _connectionLock = new(1, 1);

    private IConnection? _connection;
    private bool _disposed;

    public RabbitMqChannelManager(
        IOptions<MessageBrokerConfigurations> config,
        ILogger<RabbitMqChannelManager> logger)
    {
        var cfg = config.Value.Connection;

        _factory = new ConnectionFactory
        {
            HostName = cfg.HostName,
            Port = cfg.Port,
            UserName = cfg.UserName,
            Password = cfg.Password,
        };

        _channelOptions = new CreateChannelOptions(
            publisherConfirmationsEnabled: true,
            publisherConfirmationTrackingEnabled: true,
            outstandingPublisherConfirmationsRateLimiter: new ThrottlingRateLimiter(128)
        );

        _logger = logger;
        _exchangeName = config.Value.Exchange.Name;
    }

    public async Task<IBrokerChannel> GetChannelAsync(CancellationToken cancellationToken = default)
    {
        try
        {
            if (_disposed)
                throw new ObjectDisposedException(nameof(RabbitMqChannelManager));

            if (_connection is { IsOpen: true })
                return await CreateChannel(cancellationToken);

            await _connectionLock.WaitAsync(cancellationToken);

            if (_disposed)
                throw new ObjectDisposedException(nameof(RabbitMqChannelManager));

            if (_connection is not { IsOpen: true })
            {
                _logger.LogInformation("Creating new RabbitMQ connection...");
                _connection = await _factory.CreateConnectionAsync(cancellationToken);
            }

            return await CreateChannel(cancellationToken);
        }
        catch (BrokerUnreachableException ex)
        {
            _logger.LogError(ex, "RabbitMQ broker is unreachable.");
            throw;
        }
        catch (OperationInterruptedException ex)
        {
            _logger.LogError(ex, "Operation interrupted while creating channel.");
            throw;
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Unexpected error while creating RabbitMQ channel.");
            throw;
        }
        finally
        {
            _connectionLock.Release();
        }
    }

    private async Task<RabbitMqBrokerChannel> CreateChannel(CancellationToken cancellationToken)
    {
        var channel = await _connection!.CreateChannelAsync(_channelOptions, cancellationToken);
        return new RabbitMqBrokerChannel(channel, _exchangeName);
    }

    public async ValueTask DisposeAsync()
    {
        if (_disposed)
            return;

        _disposed = true;

        _logger.LogInformation("Disposing RabbitMQ connection manager...");

        try
        {
            if (_connection is { IsOpen: true })
                await _connection.CloseAsync();
        }
        catch (Exception ex)
        {
            _logger.LogWarning(ex, "Error occurred while closing RabbitMQ connection.");
        }

        if (_connection is not null)
            await _connection.DisposeAsync();

        _connectionLock.Dispose();
    }

}