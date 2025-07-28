using WebsiteScreenshotService.Services;
using Grpc.Core;
using WebsiteScreenshotService.Repositories;

namespace WebsiteScreenshotService;

public class GrpcScreenshotService(IAuthorizationManager authorizationManager, ISubscriptionManager subscriptionManager) : GeneratedGrpcScreenshotService.GeneratedGrpcScreenshotServiceBase
{
    private readonly IAuthorizationManager _authorizationManager = authorizationManager;

    private readonly ConfirmationResponse _invalidTokenError = new ()
    {
        Success = false,
        Message = "Invalid or expired token"
    };

    private readonly ConfirmationResponse _successfulCompletion = new ()
    {
        Success = true,
        Message = "Operation has been successfully completed"
    };

    private readonly ConfirmationResponse _errorWhileProcessing = new ()
    {
        Success = false,
        Message = "An error occurred while processing your request. Please contact administrator."
    };


    public override async Task<ConfirmationResponse> RedeemScreenshotAttempt(ConfirmationRequest request, ServerCallContext context)
        => await HandlerOperationAsync(request, async (data) => await subscriptionManager.IncrementScreenshotCountAsync(data!.UserId));

    private async Task<ConfirmationResponse> HandlerOperationAsync(ConfirmationRequest request, Func<ConfirmationData, Task> action)
    {
        try
        {
            var data = _authorizationManager.ValidateConfirmationToken(request.Token);

            if (data is null)
                return _invalidTokenError;

            await action(data);

            return _successfulCompletion;
        }
        catch
        {
            return _errorWhileProcessing;

        }
    }
}

