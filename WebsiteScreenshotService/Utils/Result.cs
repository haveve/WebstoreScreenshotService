namespace WebsiteScreenshotService.Utils;

public record Result<T>(T? Value, string? ErrorMessage) where T : class
{
    public bool IsSuccess => string.IsNullOrEmpty(ErrorMessage);

    public static Result<T> Success(T value)
        => new(value, ErrorMessage: null);

    public static Result<T> Error(string errorMessage)
        => new(Value: null, errorMessage);
}

public record Result(string? ErrorMessage)
{
    public bool IsSuccess => string.IsNullOrEmpty(ErrorMessage);

    public static Result Success => new(ErrorMessage: null);

    public static Result Error(string errorMessage)
        => new(errorMessage);
}