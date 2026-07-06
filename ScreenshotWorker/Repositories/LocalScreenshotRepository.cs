using Microsoft.Extensions.Options;
using ScreenshotWorker.Extensions;
using ScreenshotWorker.Settings;
using Shared.Core.Contracts.ScreeshotModel.Components;
using System.Net.Http.Json;

namespace ScreenshotWorker.Repositories;

public class LocalScreenshotRepository(IOptions<LocalScreenshotStorageSettings> options, IHttpClientFactory httpClientFactory) : IScreenshotRepository
{
    private readonly LocalScreenshotStorageSettings _settings = options.Value;

    public async Task<bool> SaveScreenshot(SaveScreenshotModel saveScreenshotModel, CancellationToken cancellationToken = default)
    {
        var (screenshotId, userId, screenshotData, contentType) = saveScreenshotModel;

        using var client = httpClientFactory.GetServiceRepositoryHttpClient();

        var formattedUrl = $"{_settings.Url}/screenshot/{userId}/{screenshotId}";
        var requestData = new ScreenshotData(screenshotData, GetContentType(contentType));

        var result = await client.PostAsJsonAsync(formattedUrl, requestData, cancellationToken);

        return result.IsSuccessStatusCode;
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
