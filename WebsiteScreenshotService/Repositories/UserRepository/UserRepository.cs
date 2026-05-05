using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Options;
using System.Text.Json;
using WebsiteScreenshotService.Configurations;
using WebsiteScreenshotService.Mappers.EntityMappers;
using WebsiteScreenshotService.Repositories.EF;
using WebsiteScreenshotService.Repositories.EF.DbEntities;
using WebsiteScreenshotService.Repositories.ScreenshotRepository.Models;
using WebsiteScreenshotService.Services.Security;
using WebsiteScreenshotService.Utils;

namespace WebsiteScreenshotService.Repositories.UserRepository;

/// <summary>
/// Provides an in-memory implementation of the <see cref="IUserRepository"/> interface for managing users and their subscriptions.
/// </summary>
public class UserRepository(ScreenshotDbContext context, IHashingService hashingService, IKeyService keyService, IEncryptionService encryptionService, IUserEntityMapper userEntityMapper, IOptions<EncryptionConfigurations> encryptionSettings) : IUserRepository
{
    private readonly ScreenshotDbContext _context = context;
    private readonly IHashingService _hashingService = hashingService;
    private readonly IKeyService _keyService = keyService;
    private readonly IEncryptionService _encryptionService = encryptionService;
    private readonly EncryptionConfigurations _encryptionSettings = encryptionSettings.Value;
    private readonly IUserEntityMapper _userEntityMapper = userEntityMapper;

    public async Task<Result<UserEntity>> UpdateUserAsync(Guid id, UserUpdateModel model)
    {
        var userEntity = await _context.Users
            .FirstOrDefaultAsync(u => u.Id == id);

        if (userEntity is null)
            return Result<UserEntity>.Error("User with does not exist");

        var encryptedData = _userEntityMapper.Decrypt(userEntity.EncryptedData) with { Name = model.Name };
        userEntity.EncryptedData = _userEntityMapper.Encrypt(encryptedData);

        await _context.SaveChangesAsync();

        return Result<UserEntity>.Success(userEntity);
    }

    public async Task<Result<UserEntity>> GetUserByIdAsync(Guid id)
    {
        var user = await _context.Users
            .FirstOrDefaultAsync(u => u.Id == id);

        if (user is null)
            return Result<UserEntity>.Error("User with does not exist");

        return Result<UserEntity>.Success(user);
    }

    public async Task<Result<UserEntity>> CreateUserAsync(UserCreateModel user)
    {
        var emailHash = _hashingService.Hash(user.Email);

        var exists = await _context.Users
            .AnyAsync(u => u.EmailHash == emailHash);

        if (exists)
            return Result<UserEntity>.Error("User with that email already exists");

        var salt = _keyService.GenerateBase64Key();
        var encKey = _keyService.GenerateBase64Key();

        var encryptedData = new EncryptedData(user.Name, user.Email, TotpSecret: null);

        var userEntity = new UserEntity()
        {
            Id = Guid.CreateVersion7(),
            EmailHash = emailHash,
            Salt = salt,
            PasswordHash = _hashingService.Hash(user.Password, salt),
            EncKey = _encryptionService.Encrypt(encKey, _keyService.FromBase64(_encryptionSettings.MasterKey)),
            SubscriptionPlan = new() { Type = user.SubscriptionPlan.Type, ScreenshotLeft = user.SubscriptionPlan.ScreenshotLeft },
            EncryptedData = _encryptionService.Encrypt(JsonSerializer.Serialize(encryptedData), _keyService.FromBase64(encKey))
        };

        _context.Users.Add(userEntity);
        await _context.SaveChangesAsync();

        return Result<UserEntity>.Success(userEntity);
    }

    public async Task<Result<UserEntity>> GetUserByEmailAndPasswordAsync(string email, string password)
    {
        var emailHash = _hashingService.Hash(email);
        var user = await _context.Users.FirstOrDefaultAsync(u => u.EmailHash == emailHash);

        if (user is null || !_hashingService.Verify(password, user.Salt, user.PasswordHash))
            return Result<UserEntity>.Error("User does not exist");

        return Result<UserEntity>.Success(user);
    }
}

