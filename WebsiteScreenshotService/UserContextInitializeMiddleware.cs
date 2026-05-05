using WebsiteScreenshotService.Extensions;
using WebsiteScreenshotService.Repositories.UserRepository;

namespace WebsiteScreenshotService;

public class UserContextInitializeMiddleware(ILogger<UserContextInitializeMiddleware> logger, IUserManager userManager) : IMiddleware
{
    private readonly ILogger<UserContextInitializeMiddleware> _logger = logger;
    private readonly IUserManager _userManager = userManager;
    private readonly IUserEncryptionKeyManager _encryptionKeyManager = encryptionKeyManager;

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

        var userResult = await _userManager.GetUser(userId.Value);

        if (!userResult.IsSuccess)
        {
            await httpContext.Response.UnauthorizedAccess();
            return null;
        }

        var user = userResult.Value!;
        var authType = httpContext.User.GetTokenType();

        if(authType == Constants.Claims.TokenTypes.Api)
        {
            var hash = "fgsf";
            var token = "";
        }

        var subscriptionPlan = user.SubscriptionPlan;

        var userInfo = new UserInfo(userId.Value, UserRole.User, [Permissions.User.FullAccess]);
        return new UserContext(userInfo, subscriptionPlan);
    }
}
