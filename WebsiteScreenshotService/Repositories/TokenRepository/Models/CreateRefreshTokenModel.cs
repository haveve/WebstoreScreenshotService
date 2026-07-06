using WebsiteScreenshotService.Entities;

namespace WebsiteScreenshotService.Repositories.TokenRepository.Models;

public record CreateRefreshTokenModel(
    string TokenHash,
    string FamilyId,
    DateTime Expires,
    TokenLocation IssuerLocation);
