namespace WebsiteScreenshotService.Services.Payment.Models;

public class CancelSubscriptionRequest
{
    public required string SubscriptionId { get; set; }

    public bool CancelAtPeriodEnd { get; set; }
}

