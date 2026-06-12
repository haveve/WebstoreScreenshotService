namespace WebsiteScreenshotService.Services.Admin.Models;

public sealed record AdminUserModel(
    Guid Id,
    string NickName,
    string Email,
    bool IsDisactivated,
    DateTime CreatedAt,
    string SubscriptionType,
    long SubscriptionPoints,
    List<AdminUserRefundedPaymentModel> RefundedPayments
);
