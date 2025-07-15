using WebsiteScreenshotService.Extensions;
using WebsiteScreenshotService.Repositories;

namespace WebsiteScreenshotService;

public class UserContextInitializeMiddleware(ILogger<UserContextInitializeMiddleware> logger, ISubscriptionManager subscriptionManager) : IMiddleware
{
    private readonly ILogger<UserContextInitializeMiddleware> _logger = logger;
    private readonly ISubscriptionManager _subscriptionManager = subscriptionManager;

    public async Task InvokeAsync(HttpContext context, RequestDelegate next)
    {
        if (context.User is not { Identity.IsAuthenticated: true })
        {
            await next(context);
            return;
        }

        var userContext = await CreateUserContext(context);

        if (userContext is null)
            return;

        context.SetUserContext(userContext);

        await next(context);
    }

    private async Task<UserContext?> CreateUserContext(HttpContext httpContext)
    {
        var userId = httpContext.User.GetUserId();

        if (!userId.HasValue)
        {
            await httpContext.Response.UnauthorizedAccess();
            return null;
        }

        var subscriptionPlan = await _subscriptionManager.GetUserSubscriptionAsync(userId.Value);

        if (subscriptionPlan is null)
        {
            await httpContext.Response.UnauthorizedAccess();
            return null;
        }

        return new UserContext(userId.Value, subscriptionPlan, UserRole.User);
    }
}
