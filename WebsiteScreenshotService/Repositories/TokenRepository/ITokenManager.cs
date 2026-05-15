using WebsiteScreenshotService.Entities;
using WebsiteScreenshotService.Repositories.TokenRepository.Models;
using WebsiteScreenshotService.Utils;

namespace WebsiteScreenshotService.Repositories.TokenRepository;

public interface ITokenManager
{
    Task<Result<ApiToken>> CreateApiTokenAsync(
        CreateApiTokenManagerModel model,
        Guid? userId = null);

    Task<Result> UpdateApiTokenUsageAsync(
        UpdateApiTokenUsageModel model);

    Task<Result> RevokeApiTokenAsync(
        RevokeApiTokenModel model);

    Task<Result<RefreshToken>> CreateRefreshTokenAsync(
        CreateRefreshTokenModel model,
        Guid? userId = null);

    Task<Result> RevokeRefreshTokenAsync(
        RevokeRefreshTokenModel model);

    Task<Result<RefreshToken>> GetRefreshTokenByHashAsync(
        string tokenHash);

    Task<Result<ApiToken>> GetApiTokenByHashAsync(
        string tokenHash);

    Task<Result> RevokeRefreshTokenFamilyAsync(
        RevokeRefreshTokenFamilyModel model);

    Task<Result<List<RefreshToken>>> GetAllActiveRefreshTokensAsync(
        Guid? userId = null);

    Task<Result<List<ApiToken>>> GetAllActiveApiTokensAsync(
        Guid? userId = null);
}
