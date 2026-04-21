using WebsiteScreenshotService.Entities;

namespace WebsiteScreenshotService.Repositories._EF.DbEntities;

public record SubscriptionEntity
{
    public Guid Id { get; init; }

    public Guid UserId { get; init; }

    public SubscriptionType Type { get; init; }

    public required string Provider { get; init; }

    public required string ProviderSubscriptionId { get; init; }

    public SubscriptionPeriod SubscriptionPeriod { get; init; }

    public DateTime CurrentPeriodEnd { get; set; }

    public DateTime CreatedAt { get; init; }

    public required string EncryptedData { get; set; }
}

public record EncryptedData(string PaymentMethodToken);

public enum SubscriptionPeriod
{
    Monthly,
    Yearly
}