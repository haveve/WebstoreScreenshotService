using WebsiteScreenshotService.Repositories._EF.DbEntities;

namespace WebsiteScreenshotService.Services.Admin.Models;

public record AdminLogModel(
    Guid Id,
    string Message,
    Severity Severity,
    string? Source,
    string? PropertiesJson,
    DateTime Created);
