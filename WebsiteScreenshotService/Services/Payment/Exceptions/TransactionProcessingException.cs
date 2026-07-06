namespace WebsiteScreenshotService.Services.Payment.Exceptions;

public class TransactionProcessingException : Exception
{
    public TransactionProcessingException() { }

    public TransactionProcessingException(string message)
        : base(message) { }

    public TransactionProcessingException(string message, Exception innerException)
        : base(message, innerException) { }
}
