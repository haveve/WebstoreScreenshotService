namespace WebsiteScreenshotService.Repositories._EF.DbEntities;

public class BasketLineEntity
{
    public Guid Id { get; init; }

    public Guid BasketId { get; init; }

    public Guid ProductId { get; init; }
    
    public int Quantity { get; set; }
}
