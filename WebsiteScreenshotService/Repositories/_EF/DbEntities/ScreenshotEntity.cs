using NpgsqlTypes;
using Shared.Core.Contracts.ScreeshotModel.Components;
using WebsiteScreenshotService.Entities;

namespace WebsiteScreenshotService.Repositories.EF.DbEntities;

public class ScreenshotEntity
{
    public required string Id { get; set; } 

    public required string WebsiteUrl { get; set; }

    public required Guid UserId { get; set; }

    public required DateTime CreatedAt { get; set; }

    public required ScreenshotState State { get; set; }

    public required ScreenshotType Type { get; set; }

    public string? Title { get; set; }

    public string? Description { get; set; }

    public ICollection<CategoryEntity> Categories { get; set; } = [];
}
