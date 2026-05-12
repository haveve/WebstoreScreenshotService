namespace WebsiteScreenshotService.Utils;

public record Result<T>(T? Value, string? ErrorMessage) where T : class
{
    public bool IsSuccess => string.IsNullOrEmpty(ErrorMessage);

    public static Result<T> Success(T value)
        => new(value, ErrorMessage: null);

    public static Result<T> Error(string errorMessage)
        => new(Value: null, errorMessage);
}

public record ConditionalResult(bool Value, string? ErrorMessage)
{
    public bool IsSuccess => string.IsNullOrEmpty(ErrorMessage);

    public static ConditionalResult Success(bool value)
        => new(value, ErrorMessage: null);

    public static ConditionalResult Error(string errorMessage)
        => new(Value: false, errorMessage);
}

public record IdResult(Guid Id, string? ErrorMessage)
{
    public bool IsSuccess => string.IsNullOrEmpty(ErrorMessage);

    public static IdResult Success(Guid value)
        => new(value, ErrorMessage: null);

    public static IdResult Error(string errorMessage)
        => new(Id: default, errorMessage);
}

public record Result(string? ErrorMessage)
{
    public bool IsSuccess => string.IsNullOrEmpty(ErrorMessage);

    public static Result Success => new(ErrorMessage: null);

    public static Result Error(string errorMessage)
        => new(errorMessage);
}