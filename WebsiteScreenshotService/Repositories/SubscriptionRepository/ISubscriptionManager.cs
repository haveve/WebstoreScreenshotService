using WebsiteScreenshotService.Repositories.SubscriptionRepository.Models;
using WebsiteScreenshotService.Utils;

namespace WebsiteScreenshotService.Repositories.Subscription;

public interface ISubscriptionManager
{
    /// <summary>
    /// Updates the subscription plan when a screenshot is made by the user.
    /// </summary>
    /// <returns>A task that represents the asynchronous operation. The task result contains the updated subscription plan if found; otherwise, null.</returns>
    public Task<Result> ScreenshotWasMadeAsync(MakeScreenshotModel model, Guid? userId = null);

    public Task<ConditionalResult> CanMakeScreenshotAsync(CanMakeScreenshotModel model, Guid? userId = null);

    public Task<Result> RedeemScreenshotAsync(RedeemScreenshotModel model, Guid? userId = null);

}
