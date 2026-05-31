namespace ScreenshotWorker.Exceptions;

internal class ProcessAbortException : Exception
{
    public ProcessAbortException() { }

    public ProcessAbortException(string message)
        : base(message) { }

    public ProcessAbortException(string message, Exception innerException)
        : base(message, innerException) { }
}
