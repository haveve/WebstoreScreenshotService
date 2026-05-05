using WebsiteScreenshotService.Entities;
using WebsiteScreenshotService.Repositories.EF.DbEntities;
using WebsiteScreenshotService.Repositories.ScreenshotRepository.Models;
using WebsiteScreenshotService.Utils;

namespace WebsiteScreenshotService.Repositories.UserRepository;

/// <summary>
/// Defines the contract for user repository operations, including user retrieval, creation, and subscription management.
/// </summary>
public interface IUserRepository
{
    public Task<Result<UserEntity>> UpdateUserAsync(Guid id, UserUpdateModel model);

    /// <summary>
    /// Retrieves a user by their email and password.
    /// </summary>
    /// <param name="email">The email of the user.</param>
    /// <param name="password">The password of the user.</param>
    /// <returns>A task that represents the asynchronous operation. The task result contains the user if found; otherwise, null.</returns>
    public Task<Result<UserEntity>> GetUserByEmailAndPasswordAsync(string email, string password);

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
    public Task<Result<UserEntity>> CreateUserAsync(UserCreateModel user);
}
