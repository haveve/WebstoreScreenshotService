using WebsiteScreenshotService.Repositories._EF.DbEntities.Auth;
using WebsiteScreenshotService.Repositories.TokenRepository.Models;
using WebsiteScreenshotService.Utils;

namespace WebsiteScreenshotService.Repositories.TokenRepository;

public interface ITokenRepository
{
    Task<Result<ApiTokenEntity>> CreateApiTokenAsync(
        Guid userId,
        CreateApiTokenRepositoryModel model);

    Task<Result> RevokeApiTokenAsync(
        RevokeApiTokenModel model);

    Task<Result> UpdateApiTokenUsageAsync(
        UpdateApiTokenUsageModel model);

    Task<Result<RefreshTokenEntity>> CreateRefreshTokenAsync(
        Guid userId,
        CreateRefreshTokenModel model);

    Task<Result> RevokeRefreshTokenAsync(
        RevokeRefreshTokenModel model);

    Task<Result<RefreshTokenEntity>> GetRefreshTokenByHashAsync(
        string tokenHash);

    Task<Result<ApiTokenEntity>> GetApiTokenByHashAsync(
        string tokenHash);

    Task<Result> RevokeRefreshTokenFamilyAsync(
        RevokeRefreshTokenFamilyModel model);

    Task<Result<List<RefreshTokenEntity>>> GetAllActiveRefreshTokensAsync(
        Guid userId);

    Task<Result<List<ApiTokenEntity>>> GetAllApiTokensAsync(
        Guid userId);

    Task<IdResult> GetUserIdByRefreshTokenFamilyIdAsync(
        string familyId);
}
