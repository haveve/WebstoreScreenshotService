using Microsoft.Extensions.Options;
using WebsiteScreenshotService.Settings;
using WebsiteScreenshotService.Entities;
using WebsiteScreenshotService.Model.ScreenshotOptions;

namespace WebsiteScreenshotService.Repositories.ScreenshotStorageRepository;

public class ScreenshotStorageManager(IOptions<ScreenshotStorageConfigurations> options) : IScreenshotStorageManager
{
    private readonly string baseUrl = options.Value.Url.TrimEnd('/');

    public string GetScreenshotUrl(Screenshot screenshot, Guid userId)
       => $"{baseUrl}/{userId}/{screenshot.Id}{ToImageExtension(screenshot.Type)}";

    private static string ToImageExtension(ScreenshotType type)
        => type switch
        {
            ScreenshotType.Jpeg => ".jpeg",
            ScreenshotType.Png => ".png",
            _ => throw new ArgumentOutOfRangeException(nameof(type), type, "Unsupported screenshot type")
        };

}