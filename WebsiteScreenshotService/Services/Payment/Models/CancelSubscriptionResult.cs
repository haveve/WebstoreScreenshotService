namespace WebsiteScreenshotService.Services.Payment.Models;

public class CancelSubscriptionResult
{
    public required string SubscriptionId { get; set; }

    public required CancelSubscriptionStatus Status { get; set; }
}

public enum CancelSubscriptionStatus
{
    Scheduled,   // cancel_at_period_end = true
    Canceled     // immediate delete
}