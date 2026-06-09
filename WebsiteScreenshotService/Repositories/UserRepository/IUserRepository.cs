using WebsiteScreenshotService.Entities;
using WebsiteScreenshotService.Repositories.EF.DbEntities;
using WebsiteScreenshotService.Repositories.ScreenshotRepository.Models;
using WebsiteScreenshotService.Repositories.UserRepository.Models;
using WebsiteScreenshotService.Utils;

namespace WebsiteScreenshotService.Repositories.UserRepository;

/// <summary>
/// Defines the contract for user repository operations, including user retrieval, creation, and subscription management.
/// </summary>
public interface IUserRepository
{
    public Task<Result> EnableUserAsync(Guid id);

    public Task<Result> DisableUserAsync(Guid id);

    public Task<Result> Change2faModelAsync(Guid id, Change2faRepositoryModel model);

    public Task<Result> UpdateUserPasswordAsync(Guid id, UserPasswordUpdateRepositoryModel model);

    /// <summary>
    /// Retrieves a user by their email and password.
    /// </summary>
    /// <param name="email">The email of the user.</param>
    /// <param name="password">The password of the user.</param>
    /// <returns>A task that represents the asynchronous operation. The task result contains the user if found; otherwise, null.</returns>
    public Task<Result<UserEntity>> GetUserByNickNameAsync(string nickNameHash);

    /// <summary>
    /// Retrieves a user by their unique identifier.
    /// </summary>
    /// <param name="id">The unique identifier of the user.</param>
    /// <returns>A task that represents the asynchronous operation. The task result contains the user if found; otherwise, null.</returns>
    public Task<Result<UserEntity>> GetUserByIdAsync(Guid id);

    /// <summary>
    /// Creates a new user.
    /// </summary>
    /// <param name="user">The user to create.</param>
    /// <returns>A task that represents the asynchronous operation. The task result contains the created user.</returns>
    public Task<Result<UserEntity>> CreateUserAsync(UserCreateRepositoryModel user);

    public Task<ConditionalResult> DoesUserExistWithNickNameAsync(string nickNameHash);

    Task<Result> UpdateUserSubscriptionAsync(SubscriptionPlan plan, Guid userId);
}
