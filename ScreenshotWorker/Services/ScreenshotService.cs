using WebsiteScreenshotService;

namespace ScreenshotWorker.Services;

public class ScreenshotService(GeneratedGrpcScreenshotService.GeneratedGrpcScreenshotServiceClient client) : IScreenshotService
{
    private readonly GeneratedGrpcScreenshotService.GeneratedGrpcScreenshotServiceClient _client = client;

    public async Task RedeemScreenshotAttemptAsync(string token)
         => await _client.RedeemScreenshotAttemptAsync(new()
         {
             Token = token,
         });


    public async Task ConfirmScreenshotAttemptAsync(string token)
        => await _client.ConfirmScreenshotAttemptAsync(new()
        {
            Token = token,
        });

    public async Task FailedScreenshotAttemptAsync(string token)
        => await _client.FailedScreenshotAttemptAsync(new()
        {
            Token = token,
        });
}
