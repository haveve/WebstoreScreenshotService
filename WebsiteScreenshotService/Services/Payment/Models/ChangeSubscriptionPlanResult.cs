namespace WebsiteScreenshotService.Services.Payment.Models;

public class ChangeSubscriptionPlanResult
{
    public required string SubscriptionId { get; set; }

    public ChangeSubscriptionStatus Status { get; set; } = default!;
}
public enum ChangeSubscriptionStatus
{
    UpgradeScheduled,     // upgrade at period end
    UpgradeApplied,       // immediate upgrade
    DowngradeScheduled,   // ALWAYS delayed
    AlreadyOnSamePlan,
    Failed
}