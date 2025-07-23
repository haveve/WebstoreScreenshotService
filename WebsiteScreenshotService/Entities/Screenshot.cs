namespace WebsiteScreenshotService.Entities;

public record Screenshot(string Id, string WebsiteUrl, Guid UserId, DateTime CreatedAt, string? Title = null, string? Description = null);