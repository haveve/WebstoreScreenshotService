using Shared.Core.Contracts.ScreeshotModel.Components;
using System.Text.Json.Serialization;

namespace WebsiteScreenshotService.Entities;

public record ScreenshotModel(
    string Id,
    string WebsiteUrl,
    string Url,
    DateTime CreatedAt,
    ScreenshotState State,
    [property: JsonConverter(typeof(JsonStringEnumConverter))]
    ScreenshotType Type,
    ICollection<Category> Categories,
    string? Title = null,
    string? Description = null)
{
    public ScreenshotModel(Screenshot screenshot, string Url)
        : this(screenshot.Id, screenshot.WebsiteUrl, Url, screenshot.CreatedAt, screenshot.State, screenshot.Type, screenshot.Categories, screenshot.Title, screenshot.Description)
    {
    }
}