namespace WebsiteScreenshotService.Services.Admin.Models;

public record MaintenanceModeModel(
    bool Enabled,
    DateTime UpdatedAtUtc);