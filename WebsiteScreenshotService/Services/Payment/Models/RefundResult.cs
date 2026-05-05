namespace WebsiteScreenshotService.Services.Payment.Models;

public class RefundResult
{
    public required string RefundId { get; set; }

    public required string OriginalPaymentTransactionId { get; set;  }

    public required RefundStatus Status { get; set; }
}

public enum RefundStatus
{
    Unknown = 0,

    Pending = 1,     // created but not processed yet
    Succeeded = 2,   // money returned
    Failed = 3,      // refund failed

    Canceled = 4     // rare, but possible in Stripe
}
