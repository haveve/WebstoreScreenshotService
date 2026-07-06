using WebsiteScreenshotService.Entities;
using WebsiteScreenshotService.Repositories._EF.DbEntities;

namespace WebsiteScreenshotService.Services.Payment.Models;

public class SubscriptionInfo
{
    public required Money Price { get; set; }

    public required SubscriptionPeriod Duration { get; set; }

    public required SubscriptionType SubscriptionType { get; set; }
}