using Microsoft.Extensions.Options;
using Shared.Core.Contracts.ScreeshotModel.Components;
using Shared.Core.Services;

namespace ScreenshotWorker.Repositories;

public class BlobScreenshotRepository(IOptions<BlobConfigurations> options) : IScreenshotRepository
{
    private readonly BlobStorageService _blobStorageService = new(options.Value);

    public async Task<bool> SaveScreenshot(SaveScreenshotModel saveScreenshotModel, CancellationToken cancellationToken = default)
    {
        var (screenshotId, userId, screenshotData, contentType) = saveScreenshotModel;

        var fileName = $"{userId}/{screenshotId}/{ToImageExtension(contentType)}";
        await _blobStorageService.UploadAsync(screenshotData, fileName, GetContentType(contentType));
        return true;
    }

    private static string GetContentType(ScreenshotType screenshotType)
        => screenshotType switch
        {
            ScreenshotType.Png => "image/png",
            ScreenshotType.Jpeg => "image/jpeg",
            _ => throw new ArgumentOutOfRangeException(nameof(screenshotType), screenshotType, "Unsupported screenshot type")
        };

    private static string ToImageExtension(ScreenshotType type)
        => type switch
        {
            ScreenshotType.Jpeg => ".jpeg",
            ScreenshotType.Png => ".png",
            _ => throw new ArgumentOutOfRangeException(nameof(type), type, "Unsupported screenshot type")
        };
}
