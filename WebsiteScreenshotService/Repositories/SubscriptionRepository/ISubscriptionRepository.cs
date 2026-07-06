using WebsiteScreenshotService.Repositories._EF.DbEntities;
using WebsiteScreenshotService.Repositories.SubscriptionRepository.Models;
using WebsiteScreenshotService.Utils;

namespace WebsiteScreenshotService.Repositories.Subscription;

public interface ISubscriptionRepository
{
    /// <summary>
    /// Updates the subscription plan when a screenshot is made by the user.
    /// </summary>
    /// <param name="userId">The ID of the user.</param>
    /// <returns>A task that represents the asynchronous operation. The task result contains the updated subscription plan if found; otherwise, null.</returns>
    public Task<Result> ScreenshotWasMadeAsync(MakeScreenshotModel model, Guid userId);

    public Task<ConditionalResult> CanMakeScreenshotAsync(CanMakeScreenshotModel model, Guid userId);

    public Task<Result> RedeemScreenshotAsync(RedeemScreenshotModel model, Guid userId);

    public Task<Result> AddPointsAsync(int points, Guid userId);

    Task<Result<SubscriptionEntity>> GetByProviderIdAsync(string subscriptionId);

    Task<SubscriptionEntity> AddAsync(SubscriptionEntity subscription);

    Task<Result> ProlongAsync(Guid subscriptionId, DateTime newPeriodEnd);

    Task<Result> CancelAndRemoveAsync(Guid subscriptionId);

    Task<SubscriptionEntity?> GetActiveByUserIdAsync(Guid userId);

    Task<bool> HasActiveSubscriptionAsync(Guid userId);
}
