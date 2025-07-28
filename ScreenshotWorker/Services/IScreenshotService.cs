namespace ScreenshotWorker.Services;

public interface IScreenshotService
{
    public Task RedeemScreenshotAttemptAsync(string token);
}
