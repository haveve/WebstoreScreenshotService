using Microsoft.Extensions.Options;
using ScreenshotWorker.Model;
using System.Net.Http.Json;

namespace ScreenshotWorker;

public class InMemoryScreenshotRepository(IOptions<InMemoryScreenshotStorageSettings> options, IHttpClientFactory httpClientFactory) : IScreenshotRepository
{
    private readonly InMemoryScreenshotStorageSettings _settings = options.Value;

    public async Task<bool> SaveScreenshot(string screenshotId, string userId, byte[] screenshotData, ScreenshotType contentType, CancellationToken cancellationToken = default)
    {
        using var client = httpClientFactory.CreateClient();

        var formattedUrl = $"{_settings.Url}/screenshot/{userId}/{screenshotId}";
        var requestData = new ScreenshotData(screenshotData, GetContentType(contentType));

        var result = await client.PostAsJsonAsync(formattedUrl, requestData, cancellationToken);

        return true;
    }

    private static string GetContentType(ScreenshotType screenshotType)
        => screenshotType switch
        {
            ScreenshotType.Png => "image/png",
            ScreenshotType.Jpeg => "image/jpeg",
            _ => throw new ArgumentOutOfRangeException(nameof(screenshotType), screenshotType, "Unsupported screenshot type")
        };

    private record ScreenshotData(byte[] Data, string ContentType);
}
