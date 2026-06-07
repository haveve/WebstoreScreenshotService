namespace WebsiteScreenshotService.Repositories._EF.DbEntities;

public class LogEntity
{
    public Guid Id { get; init; }

    public string Message { get; init; } = null!;

    public DateTime Created { get; init; }

    public Severity Severity { get; init; }

    public string? Source { get; init; }

    public string? PropertiesJson { get; init; }
}

public enum Severity
{
    Error = 1,
    Critical = 2,
}