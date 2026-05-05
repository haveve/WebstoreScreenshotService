namespace WebsiteScreenshotService.Services.Payment.Models;

public enum SubscriptionChangeTiming
{
    Immediate = 0,
    EndOfPeriod = 1
}

public class ChangeSubscriptionPlanRequest
{
    public required string SubscriptionId { get; set; }

    public required SubscriptionInfo NewSubscriptionInfo { get; set; }

    public required SubscriptionInfo SubscriptionInfo { get; set; }

    public SubscriptionChangeTiming UpgradeTiming { get; set; }
}
