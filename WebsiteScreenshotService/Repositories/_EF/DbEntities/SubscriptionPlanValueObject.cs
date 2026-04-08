using WebsiteScreenshotService.Entities;

namespace WebsiteScreenshotService.Repositories.EF.DbEntities;

public class SubscriptionPlanValueObject
{
    public required SubscriptionType Type { get; set; }

    public required long ScreenshotLeft { get; set; }
}
