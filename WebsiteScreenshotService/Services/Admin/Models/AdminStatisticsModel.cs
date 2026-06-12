namespace WebsiteScreenshotService.Services.Admin.Models;

public sealed record AdminStatisticsModel(
    int TotalUsers,
    int ActiveSubscriptions,
    decimal TotalRevenue,
    decimal Mrr,
    IReadOnlyList<AdminSubscriptionStatisticsModel> BySubscriptionType);
