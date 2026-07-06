namespace WebsiteScreenshotService.Repositories._EF.DbEntities;

public record OrderEntity
{
    public Guid Id { get; init; }

    public Guid UserId { get; init; }

    public decimal TotalAmount { get; init; }

    public required string Currency { get; init; }

    public OrderStatus Status { get; set; }

    public DateTime CreatedAt { get; init; }

    public List<OrderLineEntity> Lines { get; init; } = [];
}

public enum OrderStatus
{
    PendingPayment,
    Paid,
    Cancelled
}