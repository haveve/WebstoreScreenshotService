namespace WebsiteScreenshotService.Utils;

public record PaginationResult<T>(int TotalCount, IEnumerable<T> Items);