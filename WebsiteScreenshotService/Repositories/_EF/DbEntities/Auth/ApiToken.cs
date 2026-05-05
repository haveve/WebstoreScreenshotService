namespace WebsiteScreenshotService.Repositories._EF.DbEntities.Auth;

public class ApiToken
{
    public Guid Id { get; set; }

    public Guid UserId { get; set; }

    // Identity
    public required string Name { get; set; }

    public required string TokenHash { get; set; }

    public DateTime Expires { get; set; }

    public DateTime? Revoked { get; set; }

    public DateTime Created { get; set; }
    
    public DateTime? LastTimeUsed { get; set; }

    // Security tracking
    public string? CreatedIpHash { get; set; }
    
    public string? LastUsedIpHash { get; set; }

    public required string EncryptedData { get; set; }
}

public record EncryptedData(
    ICollection<string> AllowedIps, 
    string Scopes,
    string? RevokedReason);