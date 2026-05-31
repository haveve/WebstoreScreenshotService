using Shared.Core.Contracts.ScreeshotModel.Components;

namespace WebsiteScreenshotService.Repositories.ScreenshotRepository.Models;

public record ScreenshotCreateModel(string Id, string WebsiteUrl, Guid UserId, int PointsCost, ScreenshotType Type, string? Title = null, string? Description = null);