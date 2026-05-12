namespace WebsiteScreenshotService.Repositories.TokenRepository.Models;

public record RevokeRefreshTokenModel(
    Guid? ReplacedByTokenId,
    string TokenHash,
    string Reason);
