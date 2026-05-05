namespace WebsiteScreenshotService.Services.Payment.Exceptions;

public class PaymentDataProcessingException : Exception
{
    public PaymentDataProcessingException() { }

    public PaymentDataProcessingException(string message)
        : base(message) { }

    public PaymentDataProcessingException(string message, Exception innerException)
        : base(message, innerException) { }
}
