using WebsiteScreenshotService.Repositories.ScreenshotRepository.Models;

namespace WebsiteScreenshotService.Model;

public sealed record Paging(
    int Page,
    int PageSize,
    string? Query,
    SearchScope SearchScope,
    IReadOnlyCollection<Guid>? CategoryIds);