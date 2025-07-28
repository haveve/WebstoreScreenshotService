using Microsoft.Extensions.Options;
using WebsiteScreenshotService.Configurations;
using WebsiteScreenshotService.Entities;
using WebsiteScreenshotService.Model;
using WebsiteScreenshotService.Repositories;
using WebsiteScreenshotService.Services.Messaging;
using WebsiteScreenshotService.Utils;

namespace WebsiteScreenshotService.Services;

/// <summary>
/// Provides services for browser operations, including taking screenshots.
/// </summary>
public class ScreenshotService(IUserContextAccessor userContextAccessor, IMessageBrokerManager messageBrokerManager, IOptions<MessageBrokerConfigurations> options, IAuthorizationManager authorizationManager, IScreenshotManager screenshotManager) : IScreenshotService
{
    private readonly IUserContextAccessor _userContextAccessor = userContextAccessor;
    private readonly IMessageBrokerManager _messageBrokerManager = messageBrokerManager;
    private readonly IAuthorizationManager _authorizationManager = authorizationManager;
    private readonly IScreenshotManager _screenshotManager = screenshotManager;
    private readonly QueueConfig _queueConfig = options.Value.Queue;

    private readonly Result<string> defaultErrorMessage = Result<string>.Error("Failed to send screenshot request. Please, try again later");

    /// <summary>
    /// Takes a screenshot of a webpage based on the specified options.
    /// </summary>
    /// <param name="screenshotOptionsModel">The options for taking the screenshot.</param>
    /// <returns>A task that represents the asynchronous operation. The task result contains the screenshot as a stream.</returns>
    public async Task<Result<string>> MakeScreenshotAsync(ScreenshotOptionsModel screenshotOptionsModel)
    {
        var userContext = _userContextAccessor.GetCurrentUser();
        var screenshotId = MakeScreenshotId(userContext.Id);

        var confirmationToken = _authorizationManager.GenerateConfirmationToken(new ConfirmationData
            (
                UserId: userContext.Id,
                WebsiteUrl: screenshotOptionsModel.Url,
                ScreenshotId: screenshotId
            ));

        if (confirmationToken is null)
            return defaultErrorMessage;

        var model = new MakeScreenshotModel
        {
            ScreenshotId = screenshotId,
            ScreenshotOptionsModel = screenshotOptionsModel,
            ConfirmationToken = confirmationToken,
            UserInformation = new UserInformation
            {
                UserId = userContext.Id,
            }
        };

        var routingKey = _queueConfig.QueuePerSubscription[userContext.SubscriptionPlan.Type];

        await _screenshotManager.CreateAsync(new
        (
            Id: screenshotId,
            UserId: userContext.Id,
            WebsiteUrl: screenshotOptionsModel.Url,
            CreatedAt: DateTime.UtcNow,
            State: ScreenshotState.Loading
        ));

        var successfullySent = await _messageBrokerManager.SendMessageAsync(model, routingKey);

        if (!successfullySent)
        {
            await _screenshotManager.DeleteAsync(screenshotId);
            return defaultErrorMessage;
        }

        return Result<string>.Success(screenshotId);
    }

    private static string MakeScreenshotId(Guid userId)
        => $"{userId}_{DateTimeOffset.UtcNow.ToUnixTimeMilliseconds()}";
}