using WebsiteScreenshotService.Entities;
using WebsiteScreenshotService.Mappers.EntityMappers;
using WebsiteScreenshotService.Repositories.EF.DbEntities;
using WebsiteScreenshotService.Repositories.ScreenshotRepository.Models;
using WebsiteScreenshotService.Utils;

namespace WebsiteScreenshotService.Repositories.UserRepository;

/// <summary>
/// Provides an in-memory implementation of the <see cref="IUserRepository"/> interface for managing users and their subscriptions.
/// </summary>
public class UserManager(IUserRepository userRepository, IUserContextAccessor userContextAccessor, IUserEntityMapper userEntityMapper) : IUserManager
{
    private readonly IUserRepository _userRepository = userRepository;
    private readonly IUserContextAccessor _userContextAccessor = userContextAccessor;
    private readonly IUserEntityMapper _userEntityMapper = userEntityMapper;

    public async Task<Result<User>> UpdateUserAsync(Guid id, UserUpdateModel model)
    {
        var entity = await _userRepository.UpdateUserAsync(id, model);
        return FormatUserResult(entity);
    }

    public async ValueTask<Result<User>> GetUser(Guid id = default)
    {
        if (id == default)
            id = _userContextAccessor.GetCurrentUser().UserInfo.Id;

        var entity = await _userRepository.GetUserByIdAsync(id);
        return FormatUserResult(entity);
    }

    public async Task<Result<User>> CreateUserAsync(UserCreateModel user)
    {
        var entity = await _userRepository.CreateUserAsync(user);
        return FormatUserResult(entity);
    }

    public async Task<Result<User>> GetUserByEmailAndPasswordAsync(string email, string password)
    {
        var entity = await _userRepository.GetUserByEmailAndPasswordAsync(email, password);
        return FormatUserResult(entity);
    }

    private Result<User> FormatUserResult(Result<UserEntity> entity)
    {
        if (!entity.IsSuccess)
            return Result<User>.Error(entity.ErrorMessage!);

        return Result<User>.Success(_userEntityMapper.FromEntity(entity.Value!));
    }
}

