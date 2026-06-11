using WebsiteScreenshotService.Entities;
using WebsiteScreenshotService.Extensions;
using WebsiteScreenshotService.Repositories.Subscription;
using WebsiteScreenshotService.Repositories.TokenRepository;
using WebsiteScreenshotService.Repositories.UserRepository;
using WebsiteScreenshotService.Utils;

namespace WebsiteScreenshotService;

public class UserContextInitializeMiddleware(ILogger<UserContextInitializeMiddleware> logger,
    IUserManager userManager,
    ITokenManager tokenManager,
    ISubscriptionRepository subscriptionRepository) : IMiddleware
{
    private readonly ILogger<UserContextInitializeMiddleware> _logger = logger;
    private readonly ITokenManager _tokenManager = tokenManager;
    private readonly ISubscriptionRepository _subscriptionRepository = subscriptionRepository;
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

        await InitializeSubscription(userContext);
        context.SetUserContext(userContext);

        await next(context);
    }

    private async Task InitializeSubscription(UserContext userContext)
    {
        var userId = userContext.UserInfo.Id;
        var subscription = await _subscriptionRepository.GetActiveByUserIdAsync(userId);

        if (subscription is null || userContext.SubscriptionPlan.Type == subscription.Type)
            return;

        var subscriptionPlan = subscription?.Type switch
        {
            SubscriptionType.Pro => SubscriptionPlan.GetProSubscriptionPlan(),
            SubscriptionType.Advanced => SubscriptionPlan.GetProSubscriptionPlan(),
            _ => SubscriptionPlan.GetRegularSubscriptionPlan()
        };

        await _userManager.UpdateUserSubscriptionAsync(subscriptionPlan, userId);
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

        if (user.IsDisactivated)
        {
            await httpContext.Response.UserIsDisabled();
            return null;
        }

        var tokenResult = tokenVerificationResult.Value!;

        var permission = tokenResult.IsApiToken
            ? tokenResult.Permissions
            : [Permissions.User.FullAccess];

        var userInfo = new UserInfo(userId.Value, UserRole.User, permission);
        return new UserContext(userInfo, subscriptionPlan);
    }

    private async Task<Result<TokenVerificationResult>> VerifyApiTokenAsync(HttpContext httpContext)
    {
        var authType = httpContext.User.GetTokenType();

        if (authType != Constants.Claims.TokenTypes.Api)
            return Result<TokenVerificationResult>.Success(TokenVerificationResult.NotApiTokenResult);

        var userSpecificServices = httpContext.GetUserSpecificServices()!;
        var token = httpContext.GetRawAuthToken()!;

        var hash = userSpecificServices.HashingService.Hash(token);
        var storedToken = await _tokenManager.GetApiTokenByHashAsync(hash);

        if (!storedToken.IsSuccess)
        {
            await httpContext.Response.UnauthorizedAccess();
            return Result<TokenVerificationResult>.Error("Unauthorized");
        }

        var tokenValue = storedToken.Value!;

        if (tokenValue.Expires >= DateTime.UtcNow)
            return Result<TokenVerificationResult>.Success(new(Permissions: [.. tokenValue.Scopes], IsApiToken: true));

        return Result<TokenVerificationResult>.Error("Unauthorized");
    }

    private record TokenVerificationResult(string[] Permissions, bool IsApiToken)
    {
        public static TokenVerificationResult NotApiTokenResult { get; } = new(Permissions: [], IsApiToken: false);
    }
}
