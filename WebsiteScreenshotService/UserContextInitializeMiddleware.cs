using WebsiteScreenshotService.Extensions;
using WebsiteScreenshotService.Repositories.UserRepository;

namespace WebsiteScreenshotService;

public class UserContextInitializeMiddleware(ILogger<UserContextInitializeMiddleware> logger, IUserRepository userRepository) : IMiddleware
{
    private readonly ILogger<UserContextInitializeMiddleware> _logger = logger;
    private readonly IUserRepository _userRepository = userRepository;

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

        var user = await _userRepository.GetUserByIdAsync(userId.Value);

        if (user is null)
        {
            await httpContext.Response.UnauthorizedAccess();
            return null;
        }

        var subscriptionPlan = user.SubscriptionPlan;

        var userInfo = new UserInfo(userId.Value, UserRole.User);
        return new UserContext(userInfo, subscriptionPlan);
    }
}
