namespace WebsiteScreenshotService.Services.Admin.Models;

public sealed record AdminStatisticsQuery
{
    public DateTime? From { get; init; }
    public DateTime? To { get; init; }
}
