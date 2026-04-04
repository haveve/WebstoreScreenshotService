using WebsiteScreenshotService.Entities;

namespace WebsiteScreenshotService.Repositories.EF.DbEntities;

public class UserEntity
{
    public Guid Id { get; set; }
    
    public required string Email { get; set; }
    
    public required string Password { get; set; }
    
    public required string Salt { get; set; }
    
    public required string EncKey { get; set; }
    
    public required SubscriptionPlanValueObject SubscriptionPlan { get; set; }

    public required string EncryptedData { get; set; }
};

public record EncryptedData(string Name, string Email);

