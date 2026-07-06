using WebsiteScreenshotService.Services.Payment.Models;
using WebsiteScreenshotService.Utils;

namespace WebsiteScreenshotService.Services.Payment;

public interface IPaymentProviderDataProcessor
{
    Task<Result<PaymentCallbackInput>> ProcessCallbackRequestDataAsync(HttpRequest httpRequest);
}
