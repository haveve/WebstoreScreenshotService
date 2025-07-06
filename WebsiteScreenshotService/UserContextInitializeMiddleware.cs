using System.Security.Claims;
using WebsiteScreenshotService.Entities;
using WebsiteScreenshotService.Extensions;
using WebsiteScreenshotService.Repositories;

namespace WebsiteScreenshotService;

public class UserContextInitializeMiddleware(ILogger<UserContextInitializeMiddleware> logger, ISubscriptionManager subscriptionManager) : IMiddleware
{
    private readonly ILogger<UserContextInitializeMiddleware> _logger = logger;
    private readonly ISubscriptionManager _subscriptionManager = subscriptionManager;

    public async Task InvokeAsync(HttpContext context, RequestDelegate next)
    {
        if(context.User is not { Identity.IsAuthenticated: true })
        {
            await next(context);
            return;
        }   

        await next(context);
    }
}
