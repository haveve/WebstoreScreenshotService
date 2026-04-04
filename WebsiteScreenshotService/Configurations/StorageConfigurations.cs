namespace WebsiteScreenshotService.Configurations;

public class StorageConfigurations
{
   public Provider Provider { get; set; }
}

public enum Provider
{
    Postgres,
    Sqlite
}