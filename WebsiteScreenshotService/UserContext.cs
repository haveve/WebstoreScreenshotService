using WebsiteScreenshotService.Entities;

namespace WebsiteScreenshotService;

public record UserContext(UserInfo UserInfo, SubscriptionPlan SubscriptionPlan);

public enum UserRole
{
    User = 1,
}

public record UserInfo(Guid Id, UserRole Role);