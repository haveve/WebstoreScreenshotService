using WebsiteScreenshotService.Repositories._EF.DbEntities;

namespace WebsiteScreenshotService.Services.Admin.Models;

public sealed record AdminUserRefundedPaymentModel(
    Guid PaymentAttemptId,
    Guid OrderId,
    decimal Amount,
    PaymentAttemptStatus Status,
    string Provider,
    DateTime RefundedAt);