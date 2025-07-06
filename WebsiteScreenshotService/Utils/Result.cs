namespace WebsiteScreenshotService.Utils;

public record Result<T>(T? Value, string? ErrorMessage) where T : class
{
    public bool IsSuccess => string.IsNullOrEmpty(ErrorMessage);

    public static Result<T> Success(T value)
        => new(value, ErrorMessage: null);

    public static Result<T> Error(string errorMessage)
        => new(Value: null, errorMessage);
}
