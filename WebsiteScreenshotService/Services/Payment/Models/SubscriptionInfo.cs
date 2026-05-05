using WebsiteScreenshotService.Entities;

namespace WebsiteScreenshotService.Services.Payment.Models;

public class SubscriptionInfo
{
    public required Money Price { get; set; }

    public Duration Duration { get; set; }

    public SubscriptionType SubscriptionType { get; set; }
}

public enum Duration
{
    Monthly = 0,
    Yearly = 1
}
