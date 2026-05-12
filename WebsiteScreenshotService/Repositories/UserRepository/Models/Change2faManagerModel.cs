namespace WebsiteScreenshotService.Repositories.UserRepository.Models;

public record Change2faManagerModel(string? TotpSecret, IReadOnlyList<string> RecoveryCodes);