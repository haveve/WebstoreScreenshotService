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
    ConfirmationToken = 0,
    AuthorizationToken = 1,
    RefreshToken = 2,
    ApiToken = 3,
    ResetPasswordToken = 4,
    FirstAdminCreationToken = 5
}