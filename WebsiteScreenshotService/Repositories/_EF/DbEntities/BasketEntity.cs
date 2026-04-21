namespace WebsiteScreenshotService.Repositories._EF.DbEntities;

public class BasketEntity
{
    public Guid Id { get; init; }

    public Guid UserId { get; init; }

    public List<BasketLineEntity> Lines { get; init; } = [];
}
