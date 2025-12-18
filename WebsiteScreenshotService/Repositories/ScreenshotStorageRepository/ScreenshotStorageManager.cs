using Microsoft.Extensions.Options;
using ScreenshotWorker.Settings;
using WebsiteScreenshotService.Entities;
using WebsiteScreenshotService.Model;

namespace WebsiteScreenshotService.Repositories.ScreenshotStorageRepository;

public class ScreenshotStorageManager(IOptions<ScreenshotStorageConfigurations> options) : IScreenshotStorageManager
{
    private readonly string baseUrl = options.Value.Url.TrimEnd('/');

    public string GetScreenshotUrl(Screenshot screenshot)
       => $"{baseUrl}/{screenshot.UserId}/{screenshot.Id}{ToImageExtension(screenshot.Type)}";

    private static string ToImageExtension(ScreenshotType type)
        => type switch
        {
            ScreenshotType.Jpeg => ".jpeg",
            ScreenshotType.Png => ".png",
            _ => throw new ArgumentOutOfRangeException(nameof(type), type, "Unsupported screenshot type")
        };

}