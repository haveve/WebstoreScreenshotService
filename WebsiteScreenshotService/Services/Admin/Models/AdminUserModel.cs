namespace WebsiteScreenshotService.Services.Admin.Models;

public record AdminUserModel(
    Guid Id,
    string NickName,
    string Email,
    bool IsDisabled,
    DateTime CreatedAt,
    string SubscriptionType,
    long Points);
