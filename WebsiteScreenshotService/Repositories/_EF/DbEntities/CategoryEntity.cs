namespace WebsiteScreenshotService.Repositories.EF.DbEntities;

public record CategoryEntity
{
    public Guid Id { get; set; }
    
    public Guid UserId { get; set; }
    
    public string Name { get; set; } = null!;

    public string Color { get; set; } = null!;
}
