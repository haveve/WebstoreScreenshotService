namespace WebsiteScreenshotService.Repositories._EF.DbEntities;

public record PaymentAttemptEntity
{
    public Guid Id { get; init; }

    public Guid OrderId { get; init; }

    public Guid UserId { get; init; }

    public decimal Amount { get; init; }

    public PaymentAttemptStatus Status { get; set; }

    public required string Provider { get; init; }

    public string? ProviderPaymentId { get; init; }

    public bool IsPrimary { get; set; }

    public DateTime CreatedAt { get; init; }
}

public enum PaymentAttemptStatus
{
    Pending,
    Succeeded,
    Failed,
    Refunded
}