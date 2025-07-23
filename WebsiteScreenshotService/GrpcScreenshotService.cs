using WebsiteScreenshotService.Services;
using Grpc.Core;
using WebsiteScreenshotService.Repositories;

namespace WebsiteScreenshotService;

public class GrpcScreenshotService(IAuthorizationManager authorizationManager, ISubscriptionManager subscriptionManager, IScreenshotManager screenshotManager) : GeneratedGrpcScreenshotService.GeneratedGrpcScreenshotServiceBase
{
    private readonly IAuthorizationManager _authorizationManager = authorizationManager;

    public override async Task<ConfirmationResponse> ConfirmScreenshot(ConfirmationRequest request, ServerCallContext context)
        => await HandlerOperationAsync(request, async (data) => await screenshotManager.CreateAsync(new
         (
             Id: data!.ScreenshotId,
             UserId: data.UserId,
             WebsiteUrl: data.WebsiteUrl,
             CreatedAt: DateTime.UtcNow
         )));


    public override async Task<ConfirmationResponse> RedeemScreenshotAttempt(ConfirmationRequest request, ServerCallContext context)
        => await HandlerOperationAsync(request, async (data) => await subscriptionManager.IncrementScreenshotCountAsync(data!.UserId));

    private async Task<ConfirmationResponse> HandlerOperationAsync(ConfirmationRequest request, Func<ConfirmationData, Task> action)
    {
        try
        {
            var data = _authorizationManager.ValidateConfirmationToken(request.Token);

            if (data is null)
                return new ConfirmationResponse
                {
                    Success = false,
                    Message = "Invalid or expired token"
                }; ;

            await action(data);

            return new ConfirmationResponse
            {
                Success = true,
                Message = "Operation has been successfully completed"
            };
        }
        catch
        {
            return new ConfirmationResponse
            {
                Success = false,
                Message = "An error occurred while processing your request. Please contact administrator."
            };

        }
    }
}

