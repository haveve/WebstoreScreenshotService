namespace WebsiteScreenshotService.Repositories.EF.DbEntities;

public record CategoryEntity
{
    public Guid Id { get; init; }
    
    public Guid UserId { get; init; }
    
    public string Name { get; init; } = null!;

    public string Color { get; init; } = null!;
}
