namespace WebsiteScreenshotService.Repositories.TokenRepository.Models;

public record RevokeApiTokenModel(
    string TokenHash,
    string Reason);