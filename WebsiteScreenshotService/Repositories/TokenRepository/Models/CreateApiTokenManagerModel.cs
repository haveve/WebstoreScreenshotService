using WebsiteScreenshotService.Entities;

namespace WebsiteScreenshotService.Repositories.TokenRepository.Models;

public record CreateApiTokenManagerModel(string Name,
        string TokenHash,
        DateTime Expires,
        ICollection<string> Scopes,
        ICollection<string> AllowedIps,
        TokenLocation IssuerLocation);
