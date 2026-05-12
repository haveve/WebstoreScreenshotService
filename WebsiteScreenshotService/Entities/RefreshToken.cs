namespace WebsiteScreenshotService.Entities;

public record RefreshToken(
    Guid UserId,
    string TokenHash,
    string FamilyId,
    DateTime Expires,
    Guid? ReplacedByTokenId,
    string? RevokedReason,
    RefreshTokenMetadata TokenMetadata);

public record RefreshTokenMetadata(
    DateTime Issued, 
    TokenLocation IssuedLocation, 
    DateTime? Revoked, 
    TokenLocation? RevokeLocation);