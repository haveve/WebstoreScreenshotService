using Microsoft.EntityFrameworkCore;
using WebsiteScreenshotService.Repositories.EF;
using WebsiteScreenshotService.Repositories.EF.DbEntities;
using WebsiteScreenshotService.Repositories.ScreenshotRepository.Models;
using WebsiteScreenshotService.Repositories.UserRepository.Models;
using WebsiteScreenshotService.Utils;

namespace WebsiteScreenshotService.Repositories.UserRepository;

/// <summary>
/// Provides an in-memory implementation of the <see cref="IUserRepository"/> interface for managing users and their subscriptions.
/// </summary>
public class UserRepository(ScreenshotDbContext context) : IUserRepository
{
    private readonly ScreenshotDbContext _context = context;

    public async Task<Result> Change2faModelAsync(Guid id, Change2faRepositoryModel model)
    {
        var userEntity = await _context.Users
            .FirstOrDefaultAsync(u => u.Id == id);

        if (userEntity is null)
            return Result.Error("User with does not exist");

        userEntity.EncryptedData = model.EncryptedData;

        await _context.SaveChangesAsync();

        return Result.Success;
    }

    public async Task<Result> UpdateUserPasswordAsync(Guid id, UserPasswordUpdateRepositoryModel model)
    {
        var userEntity = await _context.Users
            .FirstOrDefaultAsync(u => u.Id == id);

        if (userEntity is null)
            return Result.Error("User with does not exist");

        userEntity.PasswordHash = model.PasswordHash;

        await _context.SaveChangesAsync();

        return Result.Success;
    }

    public async Task<Result<UserEntity>> GetUserByIdAsync(Guid id)
    {
        var user = await _context.Users
            .FirstOrDefaultAsync(u => u.Id == id);

        if (user is null)
            return Result<UserEntity>.Error("User with does not exist");

        return Result<UserEntity>.Success(user);
    }

    public async Task<Result<UserEntity>> CreateUserAsync(UserCreateRepositoryModel user)
    {
        var exists = (await DoesUserExistWithNickNameAsync(user.NickNameHash)).Value;

        if (exists)
            return Result<UserEntity>.Error("User with that nickname already exists");

        var userEntity = new UserEntity()
        {
            Id = user.Id,
            NickNameHash = user.NickNameHash,
            Salt = user.Salt,
            PasswordHash = user.PasswordHash,
            EncKey = user.EncKey,
            SubscriptionPlan = new() { Type = user.SubscriptionPlan.Type, Points = user.SubscriptionPlan.Points },
            EncryptedData = user.EncryptedData,
            CreatedAt = DateTime.UtcNow,
        };

        _context.Users.Add(userEntity);
        await _context.SaveChangesAsync();

        return Result<UserEntity>.Success(userEntity);
    }

    public async Task<Result<UserEntity>> GetUserByNickNameAsync(string nickNameHash)
    {
        var user = await _context.Users.FirstOrDefaultAsync(u => u.NickNameHash == nickNameHash);

        if (user is null)
            return Result<UserEntity>.Error("User does not exist");

        return Result<UserEntity>.Success(user);
    }

    public async Task<ConditionalResult> DoesUserExistWithNickNameAsync(string nickNameHash)
    {
        var exists = await _context.Users
            .AnyAsync(u => u.NickNameHash == nickNameHash);

        return ConditionalResult.Success(exists);
    }
}

