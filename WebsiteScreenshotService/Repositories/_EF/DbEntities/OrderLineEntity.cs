namespace WebsiteScreenshotService.Repositories._EF.DbEntities;

public class OrderLineEntity
{
    public Guid Id { get; init; }

    public Guid ProductId { get; init; }

    public int Quantity { get; init; }
    
    public decimal UnitPrice { get; init; }

    public required string ProductName { get; init; }
}
