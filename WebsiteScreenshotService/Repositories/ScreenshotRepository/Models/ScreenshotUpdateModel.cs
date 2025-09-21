namespace WebsiteScreenshotService.Repositories.ScreenshotRepository.Models;

public record ScreenshotUpdateModel(string Id, Guid UserId, string? Title = null, string? Description = null);