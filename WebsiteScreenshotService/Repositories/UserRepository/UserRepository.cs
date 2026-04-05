using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Options;
using System.Text.Json;
using WebsiteScreenshotService.Configurations;
using WebsiteScreenshotService.Entities;
using WebsiteScreenshotService.Repositories.EF;
using WebsiteScreenshotService.Repositories.EF.DbEntities;
using WebsiteScreenshotService.Repositories.ScreenshotRepository.Models;
using WebsiteScreenshotService.Services.Security;

namespace WebsiteScreenshotService.Repositories.UserRepository;

/// <summary>
/// Provides an in-memory implementation of the <see cref="IUserRepository"/> interface for managing users and their subscriptions.
/// </summary>
public class UserRepository(ScreenshotDbContext context, IHashingService hashingService, IKeyService keyService, IEncryptionService encryptionService, IOptions<EncryptionConfigurations> encryptionSettings) : IUserRepository
{
    private readonly ScreenshotDbContext _context = context;
    private readonly IHashingService _hashingService = hashingService;
    private readonly IKeyService _keyService = keyService;
    private readonly IEncryptionService _encryptionService = encryptionService;
    private readonly EncryptionConfigurations _encryptionSettings = encryptionSettings.Value;

    public async Task<User?> GetUserByIdAsync(Guid id)
    {
        var user = await _context.Users
            .FirstOrDefaultAsync(u => u.Id == id);

        return null;
    }

    public async Task<User?> CreateUserAsync(UserCreateModel user)
    {
        var exists = await _context.Users
            .AnyAsync(u => u.Email == user.Email);

        if (exists)
            return null;

        var salt = _keyService.GenerateBase64Key();
        var encKey = _keyService.GenerateBase64Key();

        var encryptedData = new EncryptedData(user.Name, user.Email);

        var userEntity = new UserEntity()
        {
            Id = Guid.CreateVersion7(),
            Email = _hashingService.Hash(user.Email),
            Salt = salt,
            Password = _hashingService.Hash(user.Password, salt),
            EncKey = _encryptionService.Encrypt(encKey, _keyService.FromBase64(_encryptionSettings.MasterKey)),
            SubscriptionPlan = new() { Type = user.SubscriptionPlan.Type, ScreenshotLeft = user.SubscriptionPlan.ScreenshotLeft },
            EncryptedData = _encryptionService.Encrypt(JsonSerializer.Serialize(encryptedData))
        };

        _context.Users.Add(userEntity);
        await _context.SaveChangesAsync();

        return null;
    }

    public async Task<User?> GetUserByEmailAndPasswordAsync(string email, string password)
    {
        var user = await _context.Users.FirstOrDefaultAsync(u => u.Email == email);

        if (user is null || !_hashingService.Verify(password, user.Salt, user.Password))
            return null;

        return null;
    }
}

