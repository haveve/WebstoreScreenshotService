using WebsiteScreenshotService.Model.ScreenshotOptions;

namespace WebsiteScreenshotService.Repositories.ScreenshotRepository.Models;

public record ScreenshotCreateModel(string Id, string WebsiteUrl, Guid UserId, ScreenshotType Type, string? Title = null, string? Description = null);