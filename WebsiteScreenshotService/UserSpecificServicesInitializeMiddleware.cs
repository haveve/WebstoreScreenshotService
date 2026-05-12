using Microsoft.Extensions.Options;
using WebsiteScreenshotService.Configurations;
using WebsiteScreenshotService.Extensions;
using WebsiteScreenshotService.Services.Security;

namespace WebsiteScreenshotService;

public class UserSpecificServicesInitializeMiddleware(ILogger<UserContextInitializeMiddleware> logger, IUserCryptographicDataManager cryptographicDataManager, IKeyService keyService, IEncryptionService encryptionService, IHashingService hashingService, IOptions<EncryptionConfigurations> config) : IMiddleware
{
    private readonly ILogger<UserContextInitializeMiddleware> _logger = logger;
    private readonly IUserCryptographicDataManager _cryptographicDataManager = cryptographicDataManager;
    private readonly EncryptionConfigurations _encryptionConfigurations = config.Value;
    private readonly IKeyService _keyService = keyService;
    private readonly IEncryptionService _encryptionService = encryptionService;
    private readonly IHashingService _hashingService = hashingService;

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

        var result = await _cryptographicDataManager.GetUseCryptographicDataAsync(userId.Value);

        if (!result.IsSuccess)
        {
            await httpContext.Response.UnauthorizedAccess();
            return null;
        }

        var (salt, encryptionKey) = result.Value!;

        var encryptionService = new UserEncryptionService(_keyService, _encryptionService, _encryptionConfigurations, encryptionKey, userId.Value);
        var hashingService = new UserHashingService(_hashingService, salt);

        return new()
        {
            EncryptionService = encryptionService,
            HashingService = hashingService
        };
    }
}
