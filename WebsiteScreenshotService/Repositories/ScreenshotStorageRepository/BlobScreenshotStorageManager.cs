using MassTransit.Configuration;
using Microsoft.Extensions.Options;
using Shared.Core.Contracts.ScreeshotModel.Components;
using Shared.Core.Services;
using WebsiteScreenshotService.Entities;

namespace WebsiteScreenshotService.Repositories.ScreenshotStorageRepository;

public class BlobScreenshotStorageManager (IOptions<BlobConfigurations> options) : IScreenshotStorageManager
{
    private readonly BlobStorageService blobStorageService = new(options.Value);

    public string GetScreenshotUrl(Screenshot screenshot, Guid userId)
    {
        var imagePath = $"{userId}/{screenshot.Id}{ToImageExtension(screenshot.Type)}";
        return blobStorageService.GetReadSasUrl(imagePath, TimeSpan.FromMinutes(10));
    }

    private static string ToImageExtension(ScreenshotType type)
        => type switch
        {
            ScreenshotType.Jpeg => ".jpeg",
            ScreenshotType.Png => ".png",
            _ => throw new ArgumentOutOfRangeException(nameof(type), type, "Unsupported screenshot type")
        };
}
