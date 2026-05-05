namespace WebsiteScreenshotService.Repositories._EF.DbEntities.Auth;

public class RefreshToken
{
    public Guid Id { get; set; }

    public Guid UserId { get; set; }

    // Security
    public required string TokenHash { get; set; }
    
    public required string FamilyId { get; set; }

    public required string DeviceId { get; set; }

    // IP (prefer hashed)
    public required string CreatedIpHash { get; set; }
    
    public required string LastUsedIpHash { get; set; }

    // Lifecycle
    public DateTime Created { get; set; }
    
    public DateTime Expires { get; set; }

    public DateTime? Revoked { get; set; }

    public DateTime? LastUsedAt { get; set; }

    // Rotation / security
    public Guid? ReplacedByTokenId { get; set; }

    public string? RevokedReason { get; set; }
}