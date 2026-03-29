using WebsiteScreenshotService.Model.ScreenshotOptions;

namespace WebsiteScreenshotService.Entities;

public record ScreenshotModel(string Id, string WebsiteUrl, string Url, DateTime CreatedAt, ScreenshotState State, ScreenshotType Type, string? Title = null, string? Description = null)
{
    public ScreenshotModel(Screenshot screenshot, string Url)
        : this(screenshot.Id, screenshot.WebsiteUrl, Url, screenshot.CreatedAt, screenshot.State, screenshot.Type, screenshot.Title, screenshot.Description)
    {
    }
}