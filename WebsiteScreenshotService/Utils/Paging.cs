using System.ComponentModel.DataAnnotations;

namespace WebsiteScreenshotService.Utils;

public record Paging([Range(0, 10000)] int Page, [Range(0, 10000)] int PageSize);