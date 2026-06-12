using WebsiteScreenshotService.Repositories._EF.DbEntities;

namespace WebsiteScreenshotService.Services.Admin.Models;

public record AdminPaymentAttemptModel(
    Guid Id,
    Guid OrderId,
    Guid UserId,
    decimal Amount,
    string Provider,
    string? ProviderPaymentId,
    PaymentAttemptStatus Status,
    bool IsPrimary,
    DateTime CreatedAt,
    DateTime Refunded);
