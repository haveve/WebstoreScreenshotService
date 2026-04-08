using WebsiteScreenshotService.Entities;
using WebsiteScreenshotService.Repositories.ScreenshotRepository.Models;

namespace WebsiteScreenshotService.Repositories.UserRepository;

/// <summary>
/// Provides an in-memory implementation of the <see cref="IUserRepository"/> interface for managing users and their subscriptions.
/// </summary>
public class UserManager(IUserRepository userRepository, IUserContextAccessor userContextAccessor) : IUserManager
{
    private readonly IUserRepository _userRepository = userRepository;
    private readonly IUserContextAccessor _userContextAccessor = userContextAccessor;

    public async Task<User?> UpdateUserAsync(Guid id, UserUpdateModel model)
        => await _userRepository.UpdateUserAsync(id, model);

    public async ValueTask<User?> GetUser(Guid id = default)
    {
        if (id == default)
            id = _userContextAccessor.GetCurrentUser().UserInfo.Id;

        return await _userRepository.GetUserByIdAsync(id);
    }

    public async Task<User?> CreateUserAsync(UserCreateModel user)
        => await _userRepository.CreateUserAsync(user);

    public async Task<User?> GetUserByEmailAndPasswordAsync(string email, string password)
        => await _userRepository.GetUserByEmailAndPasswordAsync(email, password);
}

