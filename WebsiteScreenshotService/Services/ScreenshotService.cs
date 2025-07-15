using Microsoft.Extensions.Options;
using WebsiteScreenshotService.Configurations;
using WebsiteScreenshotService.Model;
using WebsiteScreenshotService.Services.Messaging;
using WebsiteScreenshotService.Utils;

namespace WebsiteScreenshotService.Services;

/// <summary>
/// Provides services for browser operations, including taking screenshots.
/// </summary>
public class ScreenshotService(IUserContextAccessor userContextAccessor, IMessageBrokerProvider messageBrokerProvider, IOptions<MessageBrokerConfigurations> options) : IScreenshotService
{
    private readonly IUserContextAccessor _userContextAccessor = userContextAccessor;
    private readonly IMessageBrokerProvider _messageBrokerProvider = messageBrokerProvider;
    private readonly QueueConfig _queueConfig = options.Value.Queue;

    /// <summary>
    /// Takes a screenshot of a webpage based on the specified options.
    /// </summary>
    /// <param name="screenshotOptionsModel">The options for taking the screenshot.</param>
    /// <returns>A task that represents the asynchronous operation. The task result contains the screenshot as a stream.</returns>
    public async Task<Result<string>> MakeScreenshotAsync(ScreenshotOptionsModel screenshotOptionsModel)
    {
        var userContext = _userContextAccessor.GetCurrentUser();
        var screenshotId = MakeScreenshotId(userContext.Id);

        var model = new MakeScreenshotModel
        {
            ScreenshotId = screenshotId,
            ScreenshotOptionsModel = screenshotOptionsModel,
            UserInformation = new UserInformation
            {
                UserId = userContext.Id,
            }
        };

        var routingKey = _queueConfig.QueuePerSubscription[userContext.SubscriptionPlan.Type];

        var successfullySent = await _messageBrokerProvider.SendMessageAsync(model, routingKey);

        if (successfullySent)
            return Result<string>.Error("Failed to send screenshot request. Please, try again later");

        return Result<string>.Success(screenshotId);
    }

    private static string MakeScreenshotId(Guid userId)
        => $"{userId}_{DateTimeOffset.UtcNow.ToUnixTimeMilliseconds()}";
}