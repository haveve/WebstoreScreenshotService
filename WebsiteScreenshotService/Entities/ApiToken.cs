namespace WebsiteScreenshotService.Entities;

public record ApiToken(
    Guid UserId,
    string Name,
    string TokenHash,
    DateTime Expires,
    string? RevokedReason,
    ICollection<string> AllowedIps,
    ICollection<string> Scopes,
    ApiTokenMetadata TokenMetadata);

public record ApiTokenMetadata(
    DateTime Issued,
    TokenLocation IssuedLocation,
    DateTime? LastUsed,
    TokenLocation? LastUsedLocation,
    DateTime? Revoked,
    TokenLocation? RevokeLocation);