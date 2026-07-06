namespace WebsiteScreenshotService.Services.Payment.Models;

public class RefundRequest
{
    public required string PaymentTransactionId { get; set; }
}
