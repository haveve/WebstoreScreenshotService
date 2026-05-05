using WebsiteScreenshotService.Services.Payment.Models;
using WebsiteScreenshotService.Utils;

namespace WebsiteScreenshotService.Services.Payment;

public interface IPaymentProviderDataProcessor
{
    public Task<Result<PaymentCallbackInput>> ProcessCallbackRequestDataAsync(HttpRequest httpRequest);
}
