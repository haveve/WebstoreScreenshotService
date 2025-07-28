namespace WebsiteScreenshotService.Entities;

public record Screenshot(string Id, string WebsiteUrl, Guid UserId, DateTime CreatedAt, ScreenshotState State, string? Title = null, string? Description = null);

public enum ScreenshotState
{
    Successful = 1,
    Loading = 2,
    Failed = 3,
}