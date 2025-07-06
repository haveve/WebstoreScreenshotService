using WebsiteScreenshotService.Entities;

namespace WebsiteScreenshotService.Repositories;

public interface ISubscriptionRepository
{

    /// <summary>
    /// Retrieves the subscription plan of a user by their ID.
    /// </summary>
    /// <param name="userId">The ID of the user.</param>
    /// <returns>A task that represents the asynchronous operation. The task result contains the subscription plan if found; otherwise, null.</returns>
    public Task<SubscriptionPlan?> GetUserSubscriptionAsync(Guid userId);

    /// <summary>
    /// Updates the subscription plan when a screenshot is made by the user.
    /// </summary>
    /// <param name="userId">The ID of the user.</param>
    /// <returns>A task that represents the asynchronous operation. The task result contains the updated subscription plan if found; otherwise, null.</returns>
    public Task<SubscriptionPlan?> ScreenshotWasMadeAsync(Guid userId);

    public Task<bool> CanMakeScreenshotAsync(Guid userId);

    public Task IncrementScreenshotCountAsync(Guid userId);
}
