namespace WebsiteScreenshotService.Services.Admin.Models;

public record AdminHealthModel(
    bool DatabaseHealthy,
    bool BlobStorageHealthy,
    bool RabbitMqHealthy,
    bool StripeHealthy,
    DateTime CheckedAt);
