using WebsiteScreenshotService.Entities;

namespace WebsiteScreenshotService.Repositories;

public class SubscriptionRepository(IUserRepository userRepository): ISubscriptionRepository
{
    private readonly IUserRepository _userRepository = userRepository;

    /// <summary>
    /// Retrieves the subscription plan of a user by their ID.
    /// </summary>
    /// <param name="userId">The ID of the user.</param>
    /// <returns>A task that represents the asynchronous operation. The task result contains the subscription plan if found; otherwise, null.</returns>
    public async Task<SubscriptionPlan?> GetUserSubscriptionAsync(Guid userId)
    {
        var user = await _userRepository.GetUserByIdAsync(userId);
        return user?.SubscriptionPlan;
    }

    public async Task<bool> CanMakeScreenshotAsync(Guid userId)
    {
        var user = await _userRepository.GetUserByIdAsync(userId);

        if (user is null)
            return false;

        var subscriptionPlan = user.SubscriptionPlan;
        var canMakeScreenshot = subscriptionPlan.ScreenshotLeft > 0;

        return canMakeScreenshot;
    }

    /// <summary>
    /// Updates the subscription plan when a screenshot is made by the user.
    /// </summary>
    /// <param name="userId">The ID of the user.</param>
    /// <returns>A task that represents the asynchronous operation. The task result contains the updated subscription plan if found; otherwise, null.</returns>
    public async Task<SubscriptionPlan?> ScreenshotWasMadeAsync(Guid userId)
    {
        var user = await _userRepository.GetUserByIdAsync(userId);

        if (user is null)
            return null;

        var subscriptionPlan = user.SubscriptionPlan;

        if (subscriptionPlan.ScreenshotLeft > 0)
            subscriptionPlan.ScreenshotWasMade();

        return subscriptionPlan;
    }

    public async Task IncrementScreenshotCountAsync(Guid userId)
    {
        var user = await _userRepository.GetUserByIdAsync(userId);
        user?.SubscriptionPlan.IncrementScreenshotCount();
    }
}
