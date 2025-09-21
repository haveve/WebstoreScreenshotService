namespace WebsiteScreenshotService.Repositories.ScreenshotRepository.Models;

public record ScreenshotCreateModel(string Id, string WebsiteUrl, Guid UserId, string? Title = null, string? Description = null);