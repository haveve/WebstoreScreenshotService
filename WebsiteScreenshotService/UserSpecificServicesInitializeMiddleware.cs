using Microsoft.Extensions.Options;
using WebsiteScreenshotService.Configurations;
using WebsiteScreenshotService.Extensions;
using WebsiteScreenshotService.Services.Security;

namespace WebsiteScreenshotService;

public class UserSpecificServicesInitializeMiddleware(ILogger<UserContextInitializeMiddleware> logger, IUserEncryptionKeyManager encryptionKeyManager, IKeyService keyService, IEncryptionService encryptionService, IOptions<EncryptionConfigurations> config) : IMiddleware
{
    private readonly ILogger<UserContextInitializeMiddleware> _logger = logger;
    private readonly IUserEncryptionKeyManager _encryptionKeyManager = encryptionKeyManager;
    private readonly EncryptionConfigurations _encryptionConfigurations = config.Value;
    private readonly IKeyService _keyService = keyService;
    private readonly IEncryptionService _encryptionService = encryptionService;

    public async Task InvokeAsync(HttpContext context, RequestDelegate next)
    {
        if (context.User is not { Identity.IsAuthenticated: true })
        {
            await next(context);
            return;
        }

        var userSpecificServices = await CreateUserSpecificServices(context);

        if (userSpecificServices is null)
            return;

        context.SetUserSpecificServices(userSpecificServices);

        await next(context);
    }

    private async Task<UserSpecificServices?> CreateUserSpecificServices(HttpContext httpContext)
    {
        var userId = httpContext.User.GetUserId();

        if (!userId.HasValue)
        {
            await httpContext.Response.UnauthorizedAccess();
            return null;
        }

        var encryptionKey = await _encryptionKeyManager.GetUserEncryptionKeyAsync(userId.Value);

        if (encryptionKey is null)
        {
            await httpContext.Response.UnauthorizedAccess();
            return null;
        }

        var encryptionService = new UserEncryptionService(_keyService, _encryptionService, _encryptionConfigurations, encryptionKey);

        return new()
        {
            EncryptionService = encryptionService,
        };
    }
}
