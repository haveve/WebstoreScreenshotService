using WebsiteScreenshotService.Entities;

namespace WebsiteScreenshotService.Repositories;

public interface ISubscriptionManager
{

    /// <summary>
    /// Retrieves the subscription plan of a user by their ID.
    /// </summary>
    /// <returns>A task that represents the asynchronous operation. The task result contains the subscription plan if found; otherwise, null.</returns>
    public Task<SubscriptionPlan> GetUserSubscriptionAsync(Guid userId = default);

    /// <summary>
    /// Updates the subscription plan when a screenshot is made by the user.
    /// </summary>
    /// <returns>A task that represents the asynchronous operation. The task result contains the updated subscription plan if found; otherwise, null.</returns>
    public Task<SubscriptionPlan> ScreenshotWasMadeAsync(Guid userId = default);

    public Task<bool> CanMakeScreenshotAsync(Guid userId = default);

    public Task IncrementScreenshotCountAsync(Guid userId = default);
}
