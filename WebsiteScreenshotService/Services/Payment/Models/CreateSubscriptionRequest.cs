namespace WebsiteScreenshotService.Services.Payment.Models;

public class CreateSubscriptionRequest
{
    public required string UserId { get; set; }

    public required SubscriptionInfo SubscriptionInfo { get; set; }

    public Dictionary<string, string> Metadata { get; set; } = [];
}
