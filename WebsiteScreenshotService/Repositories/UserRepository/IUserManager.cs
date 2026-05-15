using WebsiteScreenshotService.Entities;
using WebsiteScreenshotService.Repositories.ScreenshotRepository.Models;
using WebsiteScreenshotService.Repositories.UserRepository.Models;
using WebsiteScreenshotService.Utils;

namespace WebsiteScreenshotService.Repositories.UserRepository;

/// <summary>
/// Defines the contract for user repository operations, including user retrieval, creation, and subscription management.
/// </summary>
public interface IUserManager
{
    public Task<Result> Change2faModelAsync(Change2faManagerModel model, Guid? userId = null);

    public Task<Result> UpdateUserPasswordAsync(UserPasswordUpdateManagerModel model, Guid? userId = null);

    /// <summary>
    /// Retrieves a user by their email and password.
    /// </summary>
    /// <param name="email">The email of the user.</param>
    /// <param name="password">The password of the user.</param>
    /// <returns>A task that represents the asynchronous operation. The task result contains the user if found; otherwise, null.</returns>
    public Task<Result<User>> GetUserByNickNameAndPasswordAsync(string nickName, string password);

    /// <summary>
    /// Retrieves a user by their unique identifier.
    /// </summary>
    /// <param name="id">The unique identifier of the user.</param>
    /// <returns>A task that represents the asynchronous operation. The task result contains the user if found; otherwise, null.</returns>
    public ValueTask<Result<User>> GetUser(Guid? userId = null);

    /// <summary>
    /// Creates a new user.
    /// </summary>
    /// <param name="user">The user to create.</param>
    /// <returns>A task that represents the asynchronous operation. The task result contains the created user.</returns>
    public Task<Result<User>> CreateUserAsync(UserCreateManagerModel user);

    public Task<ConditionalResult> DoesUserExistWithNickNameAsync(string nickNameHash);
}
