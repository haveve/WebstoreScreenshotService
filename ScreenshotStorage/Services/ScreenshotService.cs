namespace ScreenshotStorage.Services;

public class ScreenshotService(IWebHostEnvironment env) : IScreenshotService
{
    private readonly IWebHostEnvironment _env = env;

    public async Task SaveAsync(string userId, string screenshotId, ScreenshotData data)
    {
        string userDir = Path.Combine(_env.WebRootPath, userId);
        Directory.CreateDirectory(userDir);

        string filePath = Path.Combine(userDir, screenshotId);
        await File.WriteAllBytesAsync(filePath, data.Data);
    }

    public Task<bool> DeleteAsync(string userId, string screenshotId)
    {
        string filePath = Path.Combine(_env.WebRootPath, userId, screenshotId);
        if (File.Exists(filePath))
        {
            File.Delete(filePath);
            return Task.FromResult(true);
        }

        return Task.FromResult(false);
    }
}
