using WebsiteScreenshotService.Entities;

namespace WebsiteScreenshotService;

public record UserContext(Guid Id, SubscriptionPlan SubscriptionPlan, UserRole Role);

public enum UserRole
{
    User = 1,
}
