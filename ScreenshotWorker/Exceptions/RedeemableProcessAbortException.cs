namespace ScreenshotWorker.Exceptions;

public class RedeemableProcessAbortException : Exception
{
    public RedeemableProcessAbortException() { }

    public RedeemableProcessAbortException(string message)
        : base(message) { }

    public RedeemableProcessAbortException(string message, Exception innerException)
        : base(message, innerException) { }
}