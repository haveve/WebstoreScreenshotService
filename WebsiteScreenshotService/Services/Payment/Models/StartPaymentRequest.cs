namespace WebsiteScreenshotService.Services.Payment.Models;

public class StartPaymentRequest
{
    public Guid OrderId { get; set; }

    public required string UserId { get; set; }

    public required Money Amount { get; set; }
}
