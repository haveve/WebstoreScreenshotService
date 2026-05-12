using WebsiteScreenshotService.Extensions;
using WebsiteScreenshotService.Repositories.UserRepository;
using WebsiteScreenshotService.Utils;

namespace WebsiteScreenshotService;

public class UserContextInitializeMiddleware(ILogger<UserContextInitializeMiddleware> logger, IUserManager userManager) : IMiddleware
{
    private readonly ILogger<UserContextInitializeMiddleware> _logger = logger;
    private readonly IUserManager _userManager = userManager;

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

        var tokenVerificationResult = await VerifyApiTokenAsync(httpContext);

        if (!tokenVerificationResult.IsSuccess)
            return null;

        var userResult = await _userManager.GetUser(userId.Value);

        if (!userResult.IsSuccess)
        {
            await httpContext.Response.UnauthorizedAccess();
            return null;
        }

        var user = userResult.Value!;
        var subscriptionPlan = user.SubscriptionPlan;

        var tokenResult = tokenVerificationResult.Value!;

        var permission = tokenResult.IsApiToken
            ? tokenResult.Permissions
            : [Permissions.User.FullAccess];

        var userInfo = new UserInfo(userId.Value, UserRole.User, permission);
        return new UserContext(userInfo, subscriptionPlan);
    }

    private static async Task<Result<TokenVerificationResult>> VerifyApiTokenAsync(HttpContext httpContext)
    {
        var authType = httpContext.User.GetTokenType();

        if (authType != Constants.Claims.TokenTypes.Api)
            return Result<TokenVerificationResult>.Success(TokenVerificationResult.NotApiTokenResult);

        var userSpecificServices = httpContext.GetUserSpecificServices()!;
        var token = httpContext.GetRawAuthToken()!;

        var hash = userSpecificServices.HashingService.Hash(token);
        var valid = hash == "";

        if (!valid)
            await httpContext.Response.UnauthorizedAccess();

        return valid
            ? Result<TokenVerificationResult>.Success(new(Permissions: [], IsApiToken: true))
            : Result<TokenVerificationResult>.Error("Unauthorized");
    }

    private record TokenVerificationResult(string[] Permissions, bool IsApiToken)
    {
        public static TokenVerificationResult NotApiTokenResult { get; } = new(Permissions: [], IsApiToken: false);
    }
}
