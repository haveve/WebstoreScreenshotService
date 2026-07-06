using Microsoft.Extensions.Options;
using Shared.Core.Contracts.ScreeshotModel;
using Shared.Core.Contracts.ScreeshotModel.Components;
using WebsiteScreenshotService.Configurations;
using WebsiteScreenshotService.Entities;
using WebsiteScreenshotService.Repositories.ScreenshotRepository;
using WebsiteScreenshotService.Repositories.Subscription;
using WebsiteScreenshotService.Services.Messaging;
using WebsiteScreenshotService.Utils;

namespace WebsiteScreenshotService.Services;

/// <summary>
/// Provides services for browser operations, including taking screenshots.
/// </summary>
public class ScreenshotService(
    IUserContextAccessor userContextAccessor,
    IMessageBrokerManager messageBrokerManager,
    IOptions<MessageBrokerConfigurations> options,
    IAuthorizationManager authorizationManager,
    IScreenshotManager screenshotManager,
    ISubscriptionManager subscriptionManager,
    IScreenshotCalculator screenshotCalculator) : IScreenshotService
{
    private readonly ISubscriptionManager _subscriptionManager = subscriptionManager;
    private readonly IUserContextAccessor _userContextAccessor = userContextAccessor;
    private readonly IMessageBrokerManager _messageBrokerManager = messageBrokerManager;
    private readonly IAuthorizationManager _authorizationManager = authorizationManager;
    private readonly IScreenshotManager _screenshotManager = screenshotManager;
    private readonly IScreenshotCalculator _screenshotCalculator = screenshotCalculator;
    private readonly QueueConfig _queueConfig = options.Value.Queue;

    private readonly Result<Screenshot> defaultErrorMessage = Result<Screenshot>.Error("Failed to send screenshot request. Please, try again later");

    /// <summary>
    /// Takes a screenshot of a webpage based on the specified options.
    /// </summary>
    /// <param name="screenshotOptionsModel">The options for taking the screenshot.</param>
    /// <returns>A task that represents the asynchronous operation. The task result contains the screenshot as a stream.</returns>
    public async Task<Result<Screenshot>> MakeScreenshotAsync(ScreenshotOptionsModel screenshotOptionsModel)
    {
        var userContext = _userContextAccessor.GetCurrentUser();
        var useInfo = userContext.UserInfo;
        var screenshotPlan = userContext.SubscriptionPlan;
        var screenshotId = MakeScreenshotId();

        var pointsCost = _screenshotCalculator.CalculatePoints(screenshotOptionsModel);
        var screenshotResult = await _subscriptionManager.ScreenshotWasMadeAsync(new(pointsCost));

        if (!screenshotResult.IsSuccess)
            return Result<Screenshot>.Error(screenshotResult.ErrorMessage!);

        var confirmationToken = _authorizationManager.GenerateConfirmationToken(new ConfirmationData
            (
                UserId: useInfo.Id,
                ScreenshotId: screenshotId,
                PointsCost: pointsCost,
                TokenId: Guid.CreateVersion7().ToString("N")
            ));

        if (confirmationToken is null)
            return defaultErrorMessage;

        var savedScreenshotResult = await _screenshotManager.MakeAsync(new
        (
            Id: screenshotId,
            UserId: useInfo.Id,
            WebsiteUrl: screenshotOptionsModel.Url,
            Type: screenshotOptionsModel.ScreenshotType,
            PointsCost: pointsCost
        ));

        if (!savedScreenshotResult.IsSuccess)
            return Result<Screenshot>.Error(savedScreenshotResult.ErrorMessage!);

        var savedScreenshot = savedScreenshotResult.Value!;

        var model = new MakeScreenshotModel
        {
            ScreenshotId = savedScreenshot.Id,
            ScreenshotOptionsModel = screenshotOptionsModel,
            ConfirmationToken = confirmationToken.Token,
            UserInformation = new UserInformation
            {
                UserId = useInfo.Id,
            }
        };

        var routingKey = _queueConfig.QueuePerSubscription[screenshotPlan.Type];

        var successfullySent = await _messageBrokerManager.SendMessageAsync(model, routingKey);

        if (!successfullySent)
        {
            await _subscriptionManager.RedeemScreenshotAsync(new(savedScreenshot.Id, pointsCost));
            return defaultErrorMessage;
        }

        return Result<Screenshot>.Success(savedScreenshot);
    }

    private static string MakeScreenshotId()
        => Guid.CreateVersion7().ToString();
}