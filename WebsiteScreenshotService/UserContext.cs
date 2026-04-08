using WebsiteScreenshotService.Entities;
using WebsiteScreenshotService.Services.Security;

namespace WebsiteScreenshotService;

public record UserContext(UserInfo UserInfo, SubscriptionPlan SubscriptionPlan);

public enum UserRole
{
    User = 1,
}

public class UserSpecificServices
{
    public IUserEncryptionService EncryptionService { get; init; } = null!; 
}

public record UserInfo(Guid Id, UserRole Role);