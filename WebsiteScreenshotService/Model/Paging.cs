using System.ComponentModel.DataAnnotations;
using WebsiteScreenshotService.Repositories.ScreenshotRepository.Models;

namespace WebsiteScreenshotService.Model;

public record Paging(
    [Range(1, 10000)] int Page,
    [Range(1, 200)] int PageSize,
    [MaxLength(150)] string? Query, 
    SearchScope SearchScope,
    IReadOnlyCollection<Guid>? CategoryIds);