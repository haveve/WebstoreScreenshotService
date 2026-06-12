namespace WebsiteScreenshotService.Services.Admin.Models;

public record AdminScreenshotModel(
    string Id,
    Guid UserId,
    string WebsiteUrl,
    string? Title,
    string? Description,
    DateTime CreatedAt,
    string State,
    string Type);
