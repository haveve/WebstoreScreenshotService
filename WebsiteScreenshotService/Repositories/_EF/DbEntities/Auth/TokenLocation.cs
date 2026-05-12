namespace WebsiteScreenshotService.Repositories._EF.DbEntities.Auth;

public class TokenLocation
{
    public required string CountryCode { get; set; }

    public required string Country { get; set; }

    public string? City { get; set; }
}
