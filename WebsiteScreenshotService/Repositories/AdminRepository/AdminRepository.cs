using Microsoft.EntityFrameworkCore;
using WebsiteScreenshotService.Repositories._EF.DbEntities;
using WebsiteScreenshotService.Repositories.EF;
using WebsiteScreenshotService.Utils;

namespace WebsiteScreenshotService.Repositories.AdminRepository;

public class AdminRepository(ScreenshotDbContext context) : IAdminRepository
{
    private readonly ScreenshotDbContext _context = context;

    public async Task<ConditionalResult> DoesAnyAdminExistAsync()
    {
        var exists = await _context.Admins.AnyAsync();
        return ConditionalResult.Success(exists);
    }

    public async Task<Result<AdminEntity>> CreateAdminAsync(
        AdminCreateRepositoryModel model)
    {
        var exists = await _context.Admins
            .AnyAsync(x => x.NickNameHash == model.NickNameHash);

        if (exists)
            return Result<AdminEntity>.Error(
                "Admin with that nickname already exists.");

        var entity = new AdminEntity
        {
            Id = model.Id,
            NickNameHash = model.NickNameHash,
            PasswordHash = model.PasswordHash,
            Salt = model.Salt,
            EncKey = model.EncKey,
            EncryptedData = model.EncryptedData,
            CreatedAt = DateTime.UtcNow
        };

        _context.Admins.Add(entity);

        await _context.SaveChangesAsync();

        return Result<AdminEntity>.Success(entity);
    }

    public async Task<Result<AdminEntity>> GetAdminByCredentialsAsync(
        string nickNameHash,
        string passwordHash)
    {
        var admin = await _context.Admins
            .AsNoTracking()
            .FirstOrDefaultAsync(x =>
                x.NickNameHash == nickNameHash &&
                x.PasswordHash == passwordHash);

        if (admin is null)
            return Result<AdminEntity>.Error(
                "Invalid credentials.");

        return Result<AdminEntity>.Success(admin);
    }

    public async Task<Result<AdminEntity>> GetAdminByNickNameAsync(
        string nickNameHash)
    {
        var admin = await _context.Admins
            .AsNoTracking()
            .FirstOrDefaultAsync(x => x.NickNameHash == nickNameHash);

        if (admin is null)
            return Result<AdminEntity>.Error(
                "Admin does not exist.");

        return Result<AdminEntity>.Success(admin);
    }

    public async Task<Result<AdminEntity>> GetAdminByIdAsync(Guid id)
    {
        var admin = await _context.Admins
            .AsNoTracking()
            .FirstOrDefaultAsync(x => x.Id == id);

        if (admin is null)
            return Result<AdminEntity>.Error(
                "Admin does not exist.");

        return Result<AdminEntity>.Success(admin);
    }

    public async Task<Result> UpdatePasswordAsync(
        Guid id,
        string passwordHash)
    {
        var admin = await _context.Admins
            .FirstOrDefaultAsync(x => x.Id == id);

        if (admin is null)
            return Result.Error("Admin does not exist.");

        admin.PasswordHash = passwordHash;

        await _context.SaveChangesAsync();

        return Result.Success;
    }
}
