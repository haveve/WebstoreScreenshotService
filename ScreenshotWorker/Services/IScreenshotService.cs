namespace ScreenshotWorker.Services;

public interface IScreenshotService
{
    public Task RedeemScreenshotAttemptAsync(string token);

    public Task ConfirmScreenshotAttemptAsync(string token);

    public Task FailedScreenshotAttemptAsync(string token);
}
