using Microsoft.Extensions.Options;
using Microsoft.IdentityModel.Tokens;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;
using WebsiteScreenshotService.Configurations;
using WebsiteScreenshotService.Entities;
using WebsiteScreenshotService.Extensions;
using WebsiteScreenshotService.Repositories.ScreenshotRepository;

namespace WebsiteScreenshotService.Services;

public class AuthorizationManager(
    IOptions<AuthorizationConfiguration> options,
    ILogger<AuthorizationManager> logger,
    IScreenshotManager screenshotManager) : IAuthorizationManager
{
    private readonly AuthorizationConfiguration _config = options.Value;
    private readonly ILogger<AuthorizationManager> _logger = logger;
    private readonly IScreenshotManager _screenshotManager = screenshotManager;

    private ClaimsPrincipal? ValidateToken(string token)
    {
        try
        {
            var key = Encoding.UTF8.GetBytes(_config.Secret);

            var parameters = new TokenValidationParameters
            {
                ValidateIssuer = true,
                ValidateAudience = true,
                ValidateLifetime = true,
                ValidateIssuerSigningKey = true,
                ValidIssuer = _config.Issuer,
                ValidAudience = _config.Audience,
                IssuerSigningKey = new SymmetricSecurityKey(key),
                ClockSkew = TimeSpan.Zero
            };

            return new JwtSecurityTokenHandler()
                .ValidateToken(token, parameters, out _);
        }
        catch
        {
            return null;
        }
    }

    private static Constants.Claims.TokenTypes? GetType(ClaimsPrincipal p)
        => p.GetTokenType();

    private static Guid? GetUserId(ClaimsPrincipal p)
        => p.GetUserId();

    private static string? GetClaim(ClaimsPrincipal p, string name)
        => p.GetClaimValue(name);


    private GeneratedTokenData? GenerateToken(IEnumerable<Claim> claims, AuthorizationType type, DateTime? expires = null)
    {
        try
        {
            var expiresOn = expires.HasValue
                ? expires.Value
                : DateTime.UtcNow.AddMinutes(_config.ExpiredInMinutes[type]);

            var key = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(_config.Secret));
            var creds = new SigningCredentials(key, SecurityAlgorithms.HmacSha256);

            var token = new JwtSecurityToken(
                issuer: _config.Issuer,
                audience: _config.Audience,
                claims: claims,
                expires: expiresOn,
                signingCredentials: creds
            );

            var tokenStr = new JwtSecurityTokenHandler().WriteToken(token);

            if (tokenStr is null)
                return null;

            return new GeneratedTokenData(tokenStr, expiresOn);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Token generation failed for {Type}", type);
            return null;
        }
    }

    private static Claim C(string type, string value) => new(type, value);

    public async Task<ConfirmationData?> ValidateConfirmationToken(string token)
    {
        try
        {
            var claims = ValidateToken(token);
            if (claims is null) return null;

            if (GetType(claims) != Constants.Claims.TokenTypes.Confirmation)
                return null;

            var tokenId = GetClaim(claims, Constants.Claims.ConfirmationTokenId);
            if (string.IsNullOrWhiteSpace(tokenId))
                return null;

            if (!int.TryParse(GetClaim(claims, Constants.Claims.ScreenshotCost), out var cost))
                return null;

            var userId = GetUserId(claims);
            var screenshotId = GetClaim(claims, Constants.Claims.ScreenshotId);

            if (!userId.HasValue || string.IsNullOrWhiteSpace(screenshotId))
                return null;

            var screenshot = await _screenshotManager.GetScreenshot(screenshotId);

            if (!screenshot.IsSuccess || screenshot.Value!.State != ScreenshotState.New)
                return null;

            return new ConfirmationData(
                userId.Value,
                tokenId,
                screenshotId,
                cost);
        }
        catch
        {
            return null;
        }
    }

    public GeneratedTokenData? GenerateConfirmationToken(ConfirmationData confirmationData)
    {
        return GenerateToken(
            confirmationData.GetConfirmationTokenClaims(),
            AuthorizationType.ConfirmationToken);
    }

    public Task<ResetPassword?> ValidateResetPasswordToken(string token)
    {
        var claims = ValidateToken(token);

        if (claims is null || GetType(claims) != Constants.Claims.TokenTypes.ResetPassword)
            return Task.FromResult<ResetPassword?>(null);

        var userId = GetUserId(claims);

        if (!userId.HasValue)
            return Task.FromResult<ResetPassword?>(null);

        return Task.FromResult<ResetPassword?>(
            new ResetPassword(userId.Value));
    }

    public GeneratedTokenData? GenerateResetPasswordToken(ResetPassword data)
    {
        var claims = new[]
        {
            C(Constants.Claims.TokenType, Constants.Claims.TokenTypes.ResetPassword.ToString()),
            C(Constants.Claims.UserId, data.UserId.ToString())
        };

        return GenerateToken(claims, AuthorizationType.ResetPasswordToken);
    }

    public Task ValidateRegisterFirstAdminToken(string token)
    {
        var claims = ValidateToken(token);

        if (claims is null || GetType(claims) != Constants.Claims.TokenTypes.FirstAdminCreation)
            throw new UnauthorizedAccessException("Invalid register admin token.");

        var userId = GetUserId(claims);

        if (!userId.HasValue)
            throw new UnauthorizedAccessException("Invalid register admin token.");

        return Task.CompletedTask;
    }

    public GeneratedTokenData? GenerateRegisterFirstAdminToken()
    {
        var claims = new[]
        {
            C(Constants.Claims.TokenType, Constants.Claims.TokenTypes.FirstAdminCreation.ToString()),
        };

        return GenerateToken(claims, AuthorizationType.FirstAdminCreationToken);
    }

    public Task<RefreshData?> ValidateRefreshToken(string token)
    {
        var claims = ValidateToken(token);

        if (claims is null ||
            GetType(claims) != Constants.Claims.TokenTypes.Refresh)
            return Task.FromResult<RefreshData?>(null);

        var userId = GetUserId(claims);

        if (!userId.HasValue)
            return Task.FromResult<RefreshData?>(null);

        return Task.FromResult<RefreshData?>(
            new RefreshData(userId.Value));
    }

    public GeneratedTokenData? GenerateRefreshToken(RefreshData data)
    {
        var claims = new[]
        {
            C(Constants.Claims.TokenType, Constants.Claims.TokenTypes.Refresh.ToString()),
            C(Constants.Claims.UserId, data.UserId.ToString())
        };

        return GenerateToken(claims, AuthorizationType.RefreshToken);
    }

    public Task<AccessData?> ValidateAccessToken(string token)
    {
        var claims = ValidateToken(token);

        if (claims is null || GetType(claims) != Constants.Claims.TokenTypes.Authorization)
            return Task.FromResult<AccessData?>(null);

        var userId = GetUserId(claims);

        if (!userId.HasValue)
            return Task.FromResult<AccessData?>(null);

        return Task.FromResult<AccessData?>(
            new AccessData(userId.Value));
    }

    public GeneratedTokenData? GenerateAccessToken(AccessData data)
    {
        var claims = new[]
        {
            C(Constants.Claims.TokenType, Constants.Claims.TokenTypes.Authorization.ToString()),
            C(Constants.Claims.UserId, data.UserId.ToString()),
            C(Constants.Claims.Role, UserRoles.User)
        };

        return GenerateToken(claims, AuthorizationType.AuthorizationToken);
    }

    public Task<ApiData?> ValidateApiToken(string token)
    {
        var claims = ValidateToken(token);

        if (claims is null ||
            GetType(claims) != Constants.Claims.TokenTypes.Api)
            return Task.FromResult<ApiData?>(null);

        var userId = GetUserId(claims);
        var permissions = claims
            .FindAll(Constants.Claims.Permissions)
            .Select(c => c.Value)
            .ToArray();

        if (!userId.HasValue)
            return Task.FromResult<ApiData?>(null);

        return Task.FromResult<ApiData?>(
            new ApiData(userId.Value, permissions, null));
    }

    public GeneratedTokenData? GenerateApiToken(ApiData data)
    {
        var claims = new List<Claim>
        {
            C(Constants.Claims.TokenType, Constants.Claims.TokenTypes.Api.ToString()),
            C(Constants.Claims.UserId, data.UserId.ToString())
        };

        claims.AddRange(data.Permissions.Select(p =>
            C(Constants.Claims.Permissions, p)));

        var generatedToken = GenerateToken(claims, AuthorizationType.ApiToken, data.ExpiresOn);

        if (generatedToken is null)
            return generatedToken;

        var apiToken = $"api_v1_{generatedToken.Token}";

        return new(apiToken, generatedToken.ExpiresOn);
    }
}