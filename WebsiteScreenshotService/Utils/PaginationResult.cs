namespace WebsiteScreenshotService.Utils;

public record PaginationResult<T>(int TotalCount, IList<T> Items);