namespace ScreenshotStorage.Services;

public class ScreenshotService(IWebHostEnvironment env) : IScreenshotService
{
    private static readonly Dictionary<string, string> MimeToExtension = new(StringComparer.InvariantCultureIgnoreCase)
    {
        { "image/png", ".png" },
        { "image/jpeg", ".jpg" },
        { "image/jpg", ".jpg" },
        { "image/bmp", ".bmp" },
        { "image/gif", ".gif" },
        { "image/tiff", ".tiff" },
        { "image/webp", ".webp" },
        { "image/heic", ".heic" },
        { "image/svg+xml", ".svg" },
        { "application/pdf", ".pdf" }
    };

    private readonly IWebHostEnvironment _env = env;

    public async Task SaveAsync(string userId, string screenshotId, ScreenshotData data)
    {
        var userDir = Path.Combine(_env.WebRootPath, userId);
        Directory.CreateDirectory(userDir);

        var filePath = Path.Combine(userDir, $"{screenshotId}{MimeToExtension[data.ContentType]}");
        await File.WriteAllBytesAsync(filePath, data.Data);
    }

    public Task<bool> DeleteAsync(string userId, string screenshotId)
    {
        var filePath = Path.Combine(_env.WebRootPath, userId, screenshotId);
        var exists = File.Exists(filePath);

        if (exists)
            File.Delete(filePath);

        return Task.FromResult(exists);
    }
}
