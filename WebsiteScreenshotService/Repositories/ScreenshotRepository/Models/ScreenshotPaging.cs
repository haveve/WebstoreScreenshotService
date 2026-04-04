namespace WebsiteScreenshotService.Repositories.ScreenshotRepository.Models;

public record ScreenshotPaging(int Page, int PageSize, string? Query, SearchScope SearchScope, IReadOnlyCollection<Guid>? CategoryIds);

public enum SearchScope
{
    None = 1 << 0,
    Title = 1 << 1,
    All = 1 << 2,
}