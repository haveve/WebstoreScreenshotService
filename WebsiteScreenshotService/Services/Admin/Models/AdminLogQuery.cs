using WebsiteScreenshotService.Repositories._EF.DbEntities;

namespace WebsiteScreenshotService.Services.Admin.Models;

public sealed record AdminLogQuery
{
    public int Skip { get; init; }

    public int Take { get; init; }

    public Severity? Severity { get; init; }

    public string? Message { get; init; }

    public string? TraceId { get; init; }

    public string? Machine { get; init; }

    public string? Environment { get; init; }

    public string? JsonField { get; init; }

    public string? JsonValue { get; init; }
}
