using Microsoft.Extensions.Options;
using Microsoft.IdentityModel.Tokens;
using System.IdentityModel.Tokens.Jwt;
using System.Text;
using WebsiteScreenshotService.Configurations;
using WebsiteScreenshotService.Extensions;

namespace WebsiteScreenshotService.Services;

public class AuthorizationManager(IOptions<AuthorizationConfiguration> options, ILogger<AuthorizationManager> logger) : IAuthorizationManager
{
    private readonly AuthorizationConfiguration _config = options.Value;
    private readonly ILogger<AuthorizationManager> _logger = logger;

    public ConfirmationData? ValidateConfirmationToken(string token)
    {
        try
        {
            var key = Encoding.UTF8.GetBytes(_config.Secret);
            var validationParameters = new TokenValidationParameters
            {
                ValidateIssuer = true,
                ValidateAudience = true,
                ValidateLifetime = true,
                ValidateIssuerSigningKey = true,
                IssuerSigningKey = new SymmetricSecurityKey(key)
            };

            var tokenHandler = new JwtSecurityTokenHandler();
            var userClaims = tokenHandler.ValidateToken(token, validationParameters, out _);

            return new(
                userClaims.GetUserId()!.Value,
                userClaims.GetClaimValue(Constants.Claims.WebsiteUrl)!,
                userClaims.GetClaimValue(Constants.Claims.ScreenshotId)!
            );
        }
        catch
        {
            return null;
        }
    }

    public string? GenerateConfirmationToken(ConfirmationData confirmationData)
    {
        try
        {
            var securityKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(_config.Secret));
            var credentials = new SigningCredentials(securityKey, SecurityAlgorithms.HmacSha256);

            var token = new JwtSecurityToken(
                issuer: _config.Issuer,
                audience: _config.Audience,
                claims: confirmationData.GetConfirmationTokenClaims(),
                expires: DateTime.UtcNow.AddMinutes(_config.ExpiredInMinutes[AuthorizationType.ConfirmationToken]),
                signingCredentials: credentials
            );

            return new JwtSecurityTokenHandler().WriteToken(token);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Failed to generate confirmation token.");
            return null;
        }
    }
}

