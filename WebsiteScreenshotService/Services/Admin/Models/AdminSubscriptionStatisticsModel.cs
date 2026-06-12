namespace WebsiteScreenshotService.Services.Admin.Models;

public sealed record AdminSubscriptionStatisticsModel(
    string SubscriptionType,
    int Users,
    decimal Revenue,
    decimal Mrr);
