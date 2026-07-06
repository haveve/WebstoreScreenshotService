using Shared.Core.Contracts.ScreeshotModel.Components;
using System.Text.Json.Serialization;

namespace WebsiteScreenshotService.Entities;

public record Screenshot(string Id, string WebsiteUrl, DateTime CreatedAt, ScreenshotState State, ScreenshotType Type, ICollection<Category> Categories, string? Title = null, string? Description = null);

[JsonConverter(typeof(JsonStringEnumConverter))]
public enum ScreenshotState
{
    New = 1,
    Successful = 2,
    Failed = 3,
}