using WebsiteScreenshotService.Entities;

namespace WebsiteScreenshotService.Repositories;

/// <summary>
/// Defines the contract for user repository operations, including user retrieval, creation, and subscription management.
/// </summary>
public interface IUserRepository
{
    /// <summary>
    /// Retrieves a user by their email and password.
    /// </summary>
    /// <param name="email">The email of the user.</param>
    /// <param name="password">The password of the user.</param>
    /// <returns>A task that represents the asynchronous operation. The task result contains the user if found; otherwise, null.</returns>
    public Task<User?> GetUserByEmailAndPasswordAsync(string email, string password);

    /// <summary>
    /// Creates a new user.
    /// </summary>
    /// <param name="user">The user to create.</param>
    /// <returns>A task that represents the asynchronous operation. The task result contains the created user.</returns>
    public Task<User?> CreateUser(User user);

    /// <summary>
    /// Retrieves the subscription plan of a user by their ID.
    /// </summary>
    /// <param name="userId">The ID of the user.</param>
    /// <returns>A task that represents the asynchronous operation. The task result contains the subscription plan if found; otherwise, null.</returns>
    public Task<SubscriptionPlan?> GetUserSubscription(Guid userId);

    /// <summary>
    /// Updates the subscription plan when a screenshot is made by the user.
    /// </summary>
    /// <param name="userId">The ID of the user.</param>
    /// <returns>A task that represents the asynchronous operation. The task result contains the updated subscription plan if found; otherwise, null.</returns>
    public Task<SubscriptionPlan?> ScreenshotWasMade(Guid userId);
}
