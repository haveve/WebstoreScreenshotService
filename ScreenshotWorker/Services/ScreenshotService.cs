using Grpc.Net.Client;
using Microsoft.Extensions.Options;
using ScreenshotWorker.Settings;
using ScreenshotWorker.Utils;
using WebsiteScreenshotService;

namespace ScreenshotWorker.Services;

public class ScreenshotService(IOptions<ScreenshotServiceSettings> options) : IScreenshotService
{
    private ConcurrentLazy<GrpcChannel> _grpcChannel = new(() => GrpcChannel.ForAddress(options.Value.Url));

    public async Task RedeemScreenshotAttemptAsync(string token)
    {
        var grpcClient = new GeneratedGrpcScreenshotService.GeneratedGrpcScreenshotServiceClient(_grpcChannel.GetValue());

        await grpcClient.RedeemScreenshotAttemptAsync(new ConfirmationRequest
        {
            Token = token,
        });
    }
}
