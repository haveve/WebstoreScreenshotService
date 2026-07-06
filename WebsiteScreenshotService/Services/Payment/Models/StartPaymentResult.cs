using System.Collections.ObjectModel;

namespace WebsiteScreenshotService.Services.Payment.Models;

public record StartPaymentResult(string PaymentId, StartPaymentStatus Status, ReadOnlyDictionary<string, string> Metadata);

public enum StartPaymentStatus
{
    Pending,    // requires user action
    Succeeded,  // already paid (sync providers)
    Failed      // could not initialize payment
}