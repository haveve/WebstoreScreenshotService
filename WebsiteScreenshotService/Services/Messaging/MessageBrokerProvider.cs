namespace WebsiteScreenshotService.Services.Messaging;

public class MessageBrokerProvider(ILogger<MessageBrokerProvider> logger, IMessageBrokerChannelManager messageBrokerChannelManager) : IMessageBrokerProvider
{
    private readonly ILogger<MessageBrokerProvider> _logger = logger;

    private readonly IMessageBrokerChannelManager _messageBrokerChannelManager = messageBrokerChannelManager;

    public async Task<bool> SendMessageAsync<T>(T message, string routeKey, CancellationToken cancellationToken = default)
    {
        try
        {
            using var timeoutCts = new CancellationTokenSource(TimeSpan.FromSeconds(120));

            using var combinedCancellationToken = cancellationToken == default
                ? timeoutCts
                : CancellationTokenSource.CreateLinkedTokenSource(timeoutCts.Token, cancellationToken);

            var channel = await _messageBrokerChannelManager.GetChannelAsync(combinedCancellationToken.Token);

            await channel.PublishAsync(message, routeKey, combinedCancellationToken.Token);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error sending message");
            return false;
        }

        return true;
    }
}
