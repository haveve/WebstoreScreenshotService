namespace WebsiteScreenshotService.Entities;

public record ScreenshotModel(string Id, string WebsiteUrl, string Url, DateTime CreatedAt, ScreenshotState State, string? Title = null, string? Description = null)
{
    public ScreenshotModel(Screenshot screenshot, string Url)
        : this(screenshot.Id, screenshot.WebsiteUrl, Url, screenshot.CreatedAt, screenshot.State, screenshot.Title, screenshot.Description)
    {
    }
}