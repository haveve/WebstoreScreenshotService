using System.ComponentModel.DataAnnotations;

namespace WebsiteScreenshotService.Configurations;

public class AuthorizationConfiguration
{
    [Required]
    public required string Secret { get; set; }

    [Required]
    public required string Issuer { get; set; }

    [Required]
    public required string Audience { get; set; }

    [Required]
    public required Dictionary<AuthorizationType, int> ExpiredInMinutes { get; set; }
}

public enum AuthorizationType
{
    ConfirmationToken,
    AuthorizationToken,
    RefreshToken,
    ApiToken
}