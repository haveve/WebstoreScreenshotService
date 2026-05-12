namespace WebsiteScreenshotService.Repositories._EF.DbEntities.Auth;

public class RefreshTokenEntity
{
    public Guid Id { get; set; }

    public Guid UserId { get; set; }

    public required string TokenHash { get; set; }
    
    public required string FamilyId { get; set; }

    public DateTime Expires { get; set; }

    public required RefreshTokenMetadata TokenMetadata { get; set; }

    public Guid? ReplacedByTokenId { get; set; }

    public string? RevokedReason { get; set; }
}

public class RefreshTokenMetadata
{
    public DateTime Issued { get; set; }

    public required TokenLocation IssuedLocation { get; set; }

    public DateTime? Revoked { get; set; }

    public TokenLocation? RevokeLocation { get; set; }
}