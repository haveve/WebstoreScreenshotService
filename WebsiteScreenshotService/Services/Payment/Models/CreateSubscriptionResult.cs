namespace WebsiteScreenshotService.Services.Payment.Models;

public record CreateSubscriptionResult(string SubscriptionId, CreateSubscriptionStatus Status, IReadOnlyDictionary<string, string> Metadata);

public enum CreateSubscriptionStatus
{
    Created,                // new subscription created, needs payment
    AlreadyActive,          // user already has active subscription
    AlreadyExistsIncomplete // user has existing incomplete subscription
}