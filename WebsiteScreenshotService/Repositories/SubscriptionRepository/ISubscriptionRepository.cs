using WebsiteScreenshotService.Utils;

namespace WebsiteScreenshotService.Repositories.Subscription;

public interface ISubscriptionRepository
{
    /// <summary>
    /// Updates the subscription plan when a screenshot is made by the user.
    /// </summary>
    /// <param name="userId">The ID of the user.</param>
    /// <returns>A task that represents the asynchronous operation. The task result contains the updated subscription plan if found; otherwise, null.</returns>
    public Task<Result> ScreenshotWasMadeAsync(Guid userId);

    public Task<ConditionalResult> CanMakeScreenshotAsync(Guid userId);

    public Task<Result> RedeemScreenshotAsync(string screenshotId, Guid userId);
}
