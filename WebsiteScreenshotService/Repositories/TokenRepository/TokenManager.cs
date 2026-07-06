using WebsiteScreenshotService.Entities;
using WebsiteScreenshotService.Mappers.EntityMappers;
using WebsiteScreenshotService.Repositories._EF.DbEntities.Auth;
using WebsiteScreenshotService.Repositories.TokenRepository.Models;
using WebsiteScreenshotService.Services.Caching;
using WebsiteScreenshotService.Services.Caching.Services;
using WebsiteScreenshotService.Utils;

namespace WebsiteScreenshotService.Repositories.TokenRepository;

public class TokenManager(
    ITokenRepository tokenRepository,
    IUserContextAccessor userContextAccessor,
    IApiTokenEntityMapper apiTokenEntityMapper,
    IRefreshTokenEntityMapper refreshTokenEntityMapper,
    ICacheManager cache,
    IApiTokenCacheService apiTokenCache,
    IRefreshTokenCacheService refreshTokenCache)
    : ITokenManager
{
    private readonly ITokenRepository _tokenRepository = tokenRepository;
    private readonly IUserContextAccessor _userContextAccessor = userContextAccessor;
    private readonly IRefreshTokenEntityMapper _refreshTokenEntityMapper = refreshTokenEntityMapper;
    private readonly IApiTokenEntityMapper _apiTokenEntityMapper = apiTokenEntityMapper;
    private readonly ICacheManager _cache = cache;
    private readonly IApiTokenCacheService _apiTokenCache = apiTokenCache;
    private readonly IRefreshTokenCacheService _refreshTokenCache = refreshTokenCache;

    public async Task<Result<ApiToken>> CreateApiTokenAsync(
        CreateApiTokenManagerModel model,
        Guid? userId = null)
    {
        userId ??= _userContextAccessor.GetCurrentUser().UserInfo.Id;

        var encryptedData = new ApiTokenEncryptedData(
            model.AllowedIps,
            model.Scopes);

        var repositoryModel = new CreateApiTokenRepositoryModel(
            model.Name,
            model.TokenHash,
            model.Expires,
            _apiTokenEntityMapper.Encrypt(encryptedData),
            model.IssuerLocation);

        var result = await _tokenRepository.CreateApiTokenAsync(
            userId.Value,
            repositoryModel);

        if (!result.IsSuccess)
            return Result<ApiToken>.Error(result.ErrorMessage!);

        return Result<ApiToken>.Success(_apiTokenEntityMapper.FromEntity(result.Value!));
    }

    public async Task<Result> RevokeApiTokenAsync(
        RevokeApiTokenModel model)
    {
        var result = await _tokenRepository.RevokeApiTokenAsync(model);

        if (result.IsSuccess)
        {
            var entity = await GetApiTokenByHashAsync(model.TokenHash);

            if (!entity.IsSuccess)
                return Result.Error(entity.ErrorMessage!);

            await InvalidateApiTokenCacheAsync(entity.Value!.UserId);
        }

        return result;
    }

    public async Task<Result<RefreshToken>> CreateRefreshTokenAsync(
        CreateRefreshTokenModel model,
        Guid? userId = null)
    {
        userId ??= _userContextAccessor.GetCurrentUser().UserInfo.Id;

        var result = await _tokenRepository.CreateRefreshTokenAsync(
            userId.Value,
            model);

        if (!result.IsSuccess)
            return Result<RefreshToken>.Error(result.ErrorMessage!);

        return Result<RefreshToken>.Success(
            _refreshTokenEntityMapper.FromEntity(result.Value!));
    }

    public async Task<Result> RevokeRefreshTokenAsync(
        RevokeRefreshTokenModel model)
    {
        var result = await _tokenRepository.RevokeRefreshTokenAsync(model);

        if (result.IsSuccess)
        {
            var entity = await GetApiTokenByHashAsync(model.TokenHash);

            if (!entity.IsSuccess)
                return Result.Error(entity.ErrorMessage!);

            await InvalidateApiTokenCacheAsync(entity.Value!.UserId);
        }

        return result;
    }

    public async Task<Result<RefreshToken>> GetRefreshTokenByHashAsync(
        string tokenHash)
    {
        var key = _refreshTokenCache.ByHash(tokenHash);

        var result = await _cache.GetOrSetAsync(
            key,
            () => _tokenRepository.GetRefreshTokenByHashAsync(tokenHash),
            CacheOptions.Token.Entry);

        if (!result.IsSuccess)
            return Result<RefreshToken>.Error(result.ErrorMessage!);

        return Result<RefreshToken>.Success(
            _refreshTokenEntityMapper.FromEntity(result.Value!));
    }

    public async Task<Result<ApiToken>> GetApiTokenByHashAsync(
        string tokenHash)
    {
        var key = _apiTokenCache.ByHash(tokenHash);

        var result = await _cache.GetOrSetAsync(
            key,
            () => _tokenRepository.GetApiTokenByHashAsync(tokenHash),
            CacheOptions.Token.Entry);

        if (!result.IsSuccess)
            return Result<ApiToken>.Error(result.ErrorMessage!);

        return Result<ApiToken>.Success(
            _apiTokenEntityMapper.FromEntity(result.Value!));
    }

    public async Task<Result> RevokeRefreshTokenFamilyAsync(
        RevokeRefreshTokenFamilyModel model)
    {
        var result = await _tokenRepository
            .RevokeRefreshTokenFamilyAsync(model);

        if (result.IsSuccess)
        {
            var userIdResult = await _tokenRepository.GetUserIdByRefreshTokenFamilyIdAsync(model.FamilyId);

            if (!userIdResult.IsSuccess)
                return Result.Error(userIdResult.ErrorMessage!);

            await InvalidateRefreshTokenCacheAsync(userIdResult.Id);
        }

        return result;
    }

    public async Task<Result<List<RefreshToken>>> GetAllActiveRefreshTokensAsync(
        Guid? userId = null)
    {
        userId ??= _userContextAccessor.GetCurrentUser().UserInfo.Id;

        var key = _refreshTokenCache.ActiveList(userId.Value);

        return await _cache.GetOrSetAsync(
            key,
            async () =>
            {
                var result = await _tokenRepository
                    .GetAllActiveRefreshTokensAsync(userId.Value);

                if (!result.IsSuccess)
                    return Result<List<RefreshToken>>
                        .Error(result.ErrorMessage!);

                var list = result.Value!
                    .Select(_refreshTokenEntityMapper.FromEntity)
                    .ToList();

                return Result<List<RefreshToken>>
                    .Success(list);
            },
            CacheOptions.Token.List);
    }

    public async Task<Result<List<ApiToken>>> GetAllApiTokensAsync(
        Guid? userId = null)
    {
        userId ??= _userContextAccessor.GetCurrentUser().UserInfo.Id;

        var key = _apiTokenCache.ActiveList(userId.Value);

        return await _cache.GetOrSetAsync(
            key,
            async () =>
            {
                var result = await _tokenRepository
                    .GetAllApiTokensAsync(userId.Value);

                if (!result.IsSuccess)
                    return Result<List<ApiToken>>
                        .Error(result.ErrorMessage!);

                var list = result.Value!
                    .Select(_apiTokenEntityMapper.FromEntity)
                    .ToList();

                return Result<List<ApiToken>>
                    .Success(list);
            },
            CacheOptions.Token.List);
    }

    public async Task<Result> UpdateApiTokenUsageAsync(UpdateApiTokenUsageModel model)
    {
        var result = await _tokenRepository
            .UpdateApiTokenUsageAsync(model);

        if (result.IsSuccess)
        {
            var entity = await GetApiTokenByHashAsync(model.TokenHash);

            if (!entity.IsSuccess)
                return Result.Error(entity.ErrorMessage!);

            await InvalidateApiTokenCacheAsync(entity.Value!.UserId);
        }

        return result;
    }

    private async Task InvalidateApiTokenCacheAsync(Guid userId)
    {
        await _apiTokenCache.InvalidateUserAsync(userId);
    }

    private async Task InvalidateRefreshTokenCacheAsync(Guid userId)
    {
        await _refreshTokenCache.InvalidateUserAsync(userId);
    }
}