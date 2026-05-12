using Microsoft.EntityFrameworkCore;
using WebsiteScreenshotService.Repositories._EF.DbEntities.Auth;
using WebsiteScreenshotService.Repositories.EF;
using WebsiteScreenshotService.Repositories.TokenRepository.Models;
using WebsiteScreenshotService.Utils;

namespace WebsiteScreenshotService.Repositories.TokenRepository;

public class TokenRepository(ScreenshotDbContext context) : ITokenRepository
{
    private readonly ScreenshotDbContext _context = context;

    public async Task<Result<ApiTokenEntity>> CreateApiTokenAsync(
        Guid userId,
        CreateApiTokenRepositoryModel model)
    {
        var tokenMetadata = new ApiTokenMetadata()
        {
            Issued = DateTime.UtcNow,
            IssuedLocation = new()
            {
                City = model.IssuerLocation.City,
                Country = model.IssuerLocation.Country,
                CountryCode = model.IssuerLocation.CountryCode,
            }
        };

        var token = new ApiTokenEntity
        {
            Id = Guid.CreateVersion7(),
            UserId = userId,
            Name = model.Name,
            TokenHash = model.TokenHash,
            Expires = model.Expires,
            TokenMetadata = tokenMetadata,
            EncryptedData = model.EncryptedData,
        };

        await _context.ApiTokens.AddAsync(token);
        await _context.SaveChangesAsync();

        return Result<ApiTokenEntity>.Success(token);
    }

    public async Task<Result> RevokeApiTokenAsync(
        RevokeApiTokenModel model)
    {
        var utcNow = DateTime.UtcNow;

        var affected = await _context.ApiTokens
            .Where(x =>
                x.TokenHash == model.TokenHash &&
                x.TokenMetadata.Revoked == null)
            .ExecuteUpdateAsync(setters => setters
                .SetProperty(x => x.TokenMetadata.Revoked, utcNow)
                .SetProperty(x => x.RevokedReason, model.Reason));

        if (affected == 0)
        {
            var exists = await _context.ApiTokens
                .AnyAsync(x => x.TokenHash == model.TokenHash);

            return exists
                ? Result.Error("API token already revoked")
                : Result.Error("API token not found");
        }

        return Result.Success;
    }

    public async Task<Result<RefreshTokenEntity>> CreateRefreshTokenAsync(
        Guid userId,
        CreateRefreshTokenModel model)
    {
        var tokenMetadata = new RefreshTokenMetadata()
        {
            Issued = DateTime.UtcNow,
            IssuedLocation = new()
            {
                City = model.IssuerLocation.City,
                Country = model.IssuerLocation.Country,
                CountryCode = model.IssuerLocation.CountryCode,
            }
        };

        var token = new RefreshTokenEntity
        {
            Id = Guid.CreateVersion7(),
            UserId = userId,
            TokenHash = model.TokenHash,
            FamilyId = model.FamilyId,
            TokenMetadata = tokenMetadata,
            Expires = model.Expires,
        };

        await _context.RefreshTokens.AddAsync(token);
        await _context.SaveChangesAsync();

        return Result<RefreshTokenEntity>.Success(token);
    }

    public async Task<Result> UpdateApiTokenUsageAsync(
        UpdateApiTokenUsageModel model)
    {
        var utcNow = DateTime.UtcNow;

        var affected = await _context.ApiTokens
            .Where(x =>
                x.TokenHash == model.TokenHash &&
                x.TokenMetadata.Revoked == null)
            .ExecuteUpdateAsync(setters => setters
                .SetProperty(x => x.TokenMetadata.LastUsed, utcNow)
                .SetProperty(x => x.TokenMetadata.LastUsedLocation,
                    new TokenLocation
                    {
                        City = model.UsedLocation.City,
                        Country = model.UsedLocation.Country,
                        CountryCode = model.UsedLocation.CountryCode
                    }));

        if (affected == 0)
        {
            var exists = await _context.ApiTokens
                .AnyAsync(x => x.TokenHash == model.TokenHash);

            return exists
                ? Result.Error("API token already revoked")
                : Result.Error("API token not found");
        }

        return Result.Success;
    }

    public async Task<Result> RevokeRefreshTokenAsync(
        RevokeRefreshTokenModel model)
    {
        var utcNow = DateTime.UtcNow;

        var affected = await _context.RefreshTokens
            .Where(x =>
                x.TokenHash == model.TokenHash &&
                x.TokenMetadata.Revoked == null)
            .ExecuteUpdateAsync(setters =>
            {
                setters
                .SetProperty(x => x.TokenMetadata.Revoked, utcNow)
                .SetProperty(x => x.RevokedReason, model.Reason);

                if (model.ReplacedByTokenId.HasValue)
                    setters.SetProperty(x => x.ReplacedByTokenId, model.ReplacedByTokenId);
            });

        if (affected == 0)
        {
            var exists = await _context.RefreshTokens
                .AnyAsync(x => x.TokenHash == model.TokenHash);

            return exists
                ? Result.Error("Refresh token already revoked")
                : Result.Error("Refresh token not found");
        }

        return Result.Success;
    }

    public async Task<Result<RefreshTokenEntity>> GetRefreshTokenByHashAsync(
        string tokenHash)
    {
        var token = await _context.RefreshTokens
            .AsNoTracking()
            .FirstOrDefaultAsync(x => x.TokenHash == tokenHash);

        if (token is null)
            return Result<RefreshTokenEntity>.Error("Refresh token not found");

        return Result<RefreshTokenEntity>.Success(token);
    }

    public async Task<Result<ApiTokenEntity>> GetApiTokenByHashAsync(
        string tokenHash)
    {
        var token = await _context.ApiTokens
            .AsNoTracking()
            .FirstOrDefaultAsync(x => x.TokenHash == tokenHash);

        if (token is null)
            return Result<ApiTokenEntity>.Error("API token not found");

        return Result<ApiTokenEntity>.Success(token);
    }

    public async Task<Result> RevokeRefreshTokenFamilyAsync(
        RevokeRefreshTokenFamilyModel model)
    {
        var affected = await _context.RefreshTokens
            .Where(x =>
                x.FamilyId == model.FamilyId &&
                x.TokenMetadata.Revoked == null)
            .ExecuteUpdateAsync(setters => setters
                .SetProperty(x => x.TokenMetadata.Revoked, DateTime.UtcNow)
                .SetProperty(x => x.RevokedReason, model.Reason));

        if (affected == 0)
            return Result.Error("No active refresh tokens found");

        return Result.Success;
    }

    public async Task<IdResult> GetUserIdByRefreshTokenFamilyIdAsync(string familyId)
    {
        var userId = await _context.RefreshTokens
            .AsNoTracking()
            .Where(x => x.FamilyId == familyId)
            .Select(x => x.UserId)
            .FirstOrDefaultAsync();

        if (userId == default)
            return IdResult.Error("Specified family wasn't found");

        return IdResult.Success(userId);
    }

    public async Task<Result<List<RefreshTokenEntity>>> GetAllActiveRefreshTokensAsync(Guid userId)
    {
        var tokens = await _context.RefreshTokens
            .AsNoTracking()
            .Where(x =>
                x.UserId == userId &&
                x.TokenMetadata.Revoked == null &&
                x.Expires > DateTime.UtcNow)
            .OrderByDescending(x => x.TokenMetadata.Issued)
            .ToListAsync();

        return Result<List<RefreshTokenEntity>>.Success(tokens);
    }

    public async Task<Result<List<ApiTokenEntity>>> GetAllActiveApiTokensAsync(
        Guid userId)
    {
        var tokens = await _context.ApiTokens
            .AsNoTracking()
            .Where(x =>
                x.UserId == userId &&
                x.TokenMetadata.Revoked == null &&
                x.Expires > DateTime.UtcNow)
            .OrderByDescending(x => x.TokenMetadata.Issued)
            .ToListAsync();

        return Result<List<ApiTokenEntity>>.Success(tokens);
    }
}
