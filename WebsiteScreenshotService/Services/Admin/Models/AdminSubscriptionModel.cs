namespace WebsiteScreenshotService.Services.Admin.Models;

public record AdminSubscriptionModel(
    Guid Id,
    Guid UserId,
    string Provider,
    string ProviderSubscriptionId,
    string Type,
    string Period,
    decimal Amount,
    bool IsActive,
    DateTime CurrentPeriodEnd,
    DateTime CreatedAt);
