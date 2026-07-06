namespace WebsiteScreenshotService.Services.Admin.Models;

public record AdminPagedResult<T>(
    IReadOnlyList<T> Items,
    int TotalCount)
    where T : class;
