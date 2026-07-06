using WebsiteScreenshotService.Services;
using Grpc.Core;
using WebsiteScreenshotService.Entities;
using WebsiteScreenshotService.Repositories.ScreenshotRepository;
using WebsiteScreenshotService.Repositories.Subscription;

namespace WebsiteScreenshotService;

public class GrpcScreenshotService(IAuthorizationManager authorizationManager, ISubscriptionManager subscriptionManager, IScreenshotManager screenshotManager ,ILogger<GrpcScreenshotService> logger) : GeneratedGrpcScreenshotService.GeneratedGrpcScreenshotServiceBase
{
    private readonly ILogger<GrpcScreenshotService> _logger = logger;
    private readonly IAuthorizationManager _authorizationManager = authorizationManager;

    private readonly ConfirmationResponse _invalidTokenError = new()
    {
        Success = false,
        Message = "Invalid or expired token"
    };

    private readonly ConfirmationResponse _successfulCompletion = new()
    {
        Success = true,
        Message = "Operation has been successfully completed"
    };

    private readonly ConfirmationResponse _errorWhileProcessing = new()
    {
        Success = false,
        Message = "An error occurred while processing your request. Please contact administrator."
    };

    public override async Task<ConfirmationResponse> FailedScreenshotAttempt(ConfirmationRequest request, ServerCallContext context)
        => await HandlerOperationAsync(request, async (data) => await screenshotManager.UpdateStateAsync(data.ScreenshotId, ScreenshotState.Failed));

    public override async Task<ConfirmationResponse> ConfirmScreenshotAttempt(ConfirmationRequest request, ServerCallContext context)
        => await HandlerOperationAsync(request, async (data) => await screenshotManager.UpdateStateAsync(data.ScreenshotId, ScreenshotState.Successful));

    public override async Task<ConfirmationResponse> RedeemScreenshotAttempt(ConfirmationRequest request, ServerCallContext context)
        => await HandlerOperationAsync(request, async (data) => await subscriptionManager.RedeemScreenshotAsync(new(data.ScreenshotId, data.PointsCost), data!.UserId));

    private async Task<ConfirmationResponse> HandlerOperationAsync(ConfirmationRequest request, Func<ConfirmationData, Task> action)
    {
        try
        {
            var data = await _authorizationManager.ValidateConfirmationToken(request.Token);

            if (data is null)
                return _invalidTokenError;

            await action(data);

            return _successfulCompletion;
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "An error occurred while processing the screenshot attempt for user with token {Token}", request.Token);
            return _errorWhileProcessing;
        }
    }
}

