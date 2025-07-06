using System.Security.Claims;
using WebsiteScreenshotService.Entities;
using WebsiteScreenshotService.Extensions;

namespace WebsiteScreenshotService;

public record UserContext(Guid Id, SubscriptionPlan SubscriptionPlan ,UserRole Role);

public enum UserRole
{
    User = 1,
}
