using System.Text.Json.Serialization;
using WebsiteScreenshotService.Model;

namespace WebsiteScreenshotService.Entities;

public record Screenshot(string Id, string WebsiteUrl, Guid UserId, DateTime CreatedAt, ScreenshotState State, ScreenshotType Type, string? Title = null, string? Description = null);

[JsonConverter(typeof(JsonStringEnumConverter))]
public enum ScreenshotState
{
    New = 1,
    Successful = 2,
    Failed = 3,
}