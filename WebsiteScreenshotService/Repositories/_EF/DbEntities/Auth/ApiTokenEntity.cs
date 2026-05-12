namespace WebsiteScreenshotService.Repositories._EF.DbEntities.Auth;

public class ApiTokenEntity
{
    public Guid Id { get; set; }

    public Guid UserId { get; set; }

    public required string Name { get; set; }

    public required string TokenHash { get; set; }

    public DateTime Expires { get; set; }

    public required ApiTokenMetadata TokenMetadata { get; set; }

    public required string EncryptedData { get; set; }

    public string? RevokedReason { get; set; }
}

public record ApiTokenEncryptedData(
    ICollection<string> AllowedIps,
    ICollection<string> Scopes);

public class ApiTokenMetadata
{
    public DateTime Issued { get; set; }

    public required TokenLocation IssuedLocation { get; set; }

    public DateTime? LastUsed { get; set; }

    public TokenLocation? LastUsedLocation { get; set; }

    public DateTime? Revoked { get; set; }

    public TokenLocation? RevokeLocation { get; set; }
}