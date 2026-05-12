using WebsiteScreenshotService.Entities;

namespace WebsiteScreenshotService.Repositories.TokenRepository.Models;

public record CreateApiTokenRepositoryModel(string Name,
        string TokenHash,
        DateTime Expires,
        string EncryptedData,
        TokenLocation IssuerLocation);
