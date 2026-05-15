using Microsoft.Extensions.Options;
using System.Text.Json;
using WebsiteScreenshotService.Configurations;
using WebsiteScreenshotService.Entities;
using WebsiteScreenshotService.Mappers.EntityMappers;
using WebsiteScreenshotService.Repositories.EF.DbEntities;
using WebsiteScreenshotService.Repositories.ScreenshotRepository.Models;
using WebsiteScreenshotService.Repositories.UserRepository.Models;
using WebsiteScreenshotService.Services.Caching;
using WebsiteScreenshotService.Services.Caching.Services;
using WebsiteScreenshotService.Services.Security;
using WebsiteScreenshotService.Utils;

namespace WebsiteScreenshotService.Repositories.UserRepository;

/// <summary>
/// Provides an in-memory implementation of the <see cref="IUserRepository"/> interface for managing users and their subscriptions.
/// </summary>
public class UserManager(
    IUserRepository userRepository,
    IUserContextAccessor userContextAccessor,
    IUserEntityMapper userEntityMapper,
    IHashingService hashingService,
    IKeyService keyService,
    IEncryptionService encryptionService,
    IOptions<EncryptionConfigurations> encryptionConfig,
    ICacheManager cache,
    IUserCacheService userCache) : IUserManager
{
    private readonly IUserRepository _userRepository = userRepository;
    private readonly IUserContextAccessor _userContextAccessor = userContextAccessor;
    private readonly IUserEntityMapper _userEntityMapper = userEntityMapper;
    private readonly IHashingService _hashingService = hashingService;
    private readonly IKeyService _keyService = keyService;
    private readonly IEncryptionService _encryptionService = encryptionService;
    private readonly EncryptionConfigurations _encryptionSettings = encryptionConfig.Value;
    private readonly ICacheManager _cache = cache;
    private readonly IUserCacheService _userCache = userCache;

    public async ValueTask<Result<User>> GetUser(Guid? userId = null)
    {
        userId ??= _userContextAccessor.GetCurrentUser().UserInfo.Id;

        var entityResult = await GetUserProfileCachedAsync(userId.Value);

        return FormatUserResult(entityResult);
    }

    public async Task<Result<User>> CreateUserAsync(UserCreateManagerModel user)
    {
        var nickNameHash = _hashingService.Hash(user.NickName);

        var exists = (await _userRepository.DoesUserExistWithNickNameAsync(nickNameHash)).Value;

        if (exists)
            return Result<User>.Error("User with that nickname already exists");

        var salt = _keyService.GenerateBase64Key();
        var encKey = _keyService.GenerateBase64Key();

        var encryptedData = new UserEncryptedData(user.NickName, user.Email, TwoFactorModel: null);
        var userId = Guid.CreateVersion7();

        var userRepositoryModel = new UserCreateRepositoryModel(
            Id: userId,
            NickNameHash: nickNameHash,
            PasswordHash: _hashingService.Hash(user.Password, salt),
            Salt: salt,
            EncKey: _encryptionService.Encrypt(
                encKey,
                _keyService.FromBase64(_encryptionSettings.MasterKey),
                UserEncryptionContextFactory.ForUserEncKey(userId.ToString())
            ),
            SubscriptionPlan: user.SubscriptionPlan,
            EncryptedData: _encryptionService.Encrypt(
                JsonSerializer.Serialize(encryptedData),
                _keyService.FromBase64(encKey),
                UserEncryptionContextFactory.ForUserEncEntityData(userId.ToString())
            )
        );

        var entity = await _userRepository.CreateUserAsync(userRepositoryModel);

        return FormatUserResult(entity);
    }

    public async Task<Result> UpdateUserPasswordAsync(UserPasswordUpdateManagerModel model, Guid? userId = null)
    {
        userId ??= _userContextAccessor.GetCurrentUser().UserInfo.Id;

        var user = await GetUserProfileCachedAsync(userId.Value);

        if (!user.IsSuccess)
            return Result.Error(user.ErrorMessage!);

        var salt = user.Value!.Salt;
        var passwordHash = _hashingService.Hash(model.Password, salt);

        var repositoryModel = new UserPasswordUpdateRepositoryModel(passwordHash);

        var result = await _userRepository.UpdateUserPasswordAsync(userId.Value, repositoryModel);

        if (result.IsSuccess)
            await InvalidateUserCacheAsync(userId.Value);

        return result;
    }

    public async Task<Result> Change2faModelAsync(Change2faManagerModel model, Guid? userId = null)
    {
        userId ??= _userContextAccessor.GetCurrentUser().UserInfo.Id;

        var user = await GetUserProfileCachedAsync(userId.Value);

        if (!user.IsSuccess)
            return Result.Error(user.ErrorMessage!);

        var encryptedData = user.Value!.EncryptedData;
        var currentData = _userEntityMapper.Decrypt(encryptedData);

        var twoFactorModel = model.TotpSecret is not null
            ? new TwoFactorModel(
                model.TotpSecret,
                [.. model.RecoveryCodes.Select(c => new RecoveryCode(c, WasUsed: false))])
            : null;

        var updatedData = currentData with { TwoFactorModel = twoFactorModel };

        var repositoryModel = new Change2faRepositoryModel(
            _userEntityMapper.Encrypt(updatedData));

        var result = await _userRepository.Change2faModelAsync(userId.Value, repositoryModel);

        if (result.IsSuccess)
            await InvalidateUserCacheAsync(userId.Value);

        return result;
    }

    public async Task<Result<User>> GetUserByNickNameAndPasswordAsync(string nickName, string password)
    {
        var nickNameHash = _hashingService.Hash(nickName);
        var entity = await _userRepository.GetUserByNickNameAsync(nickNameHash);

        if (!entity.IsSuccess)
            return Result<User>.Error("User does not exist");

        if (!_hashingService.Verify(password, entity.Value!.Salt, entity.Value!.PasswordHash))
            return Result<User>.Error("User does not exist");

        return FormatUserResult(entity);
    }

    public async Task<ConditionalResult> DoesUserExistWithNickNameAsync(string nickNameHash)
    {
        return await _userRepository.DoesUserExistWithNickNameAsync(nickNameHash);
    }

    private async Task<Result<UserEntity>> GetUserProfileCachedAsync(Guid id)
    {
        var key = _userCache.Profile(id);

        var entityResult = await _cache.GetOrSetAsync(
            key,
            () => _userRepository.GetUserByIdAsync(id),
            CacheOptions.User.Entry);

        return entityResult;
    }

    private Result<User> FormatUserResult(Result<UserEntity> entity)
    {
        if (!entity.IsSuccess)
            return Result<User>.Error(entity.ErrorMessage!);

        return Result<User>.Success(
            _userEntityMapper.FromEntity(entity.Value!));
    }

    private async Task InvalidateUserCacheAsync(Guid userId)
    {
        await _cache.RemoveAsync(_userCache.Profile(userId));
    }
}
