namespace ScreenshotWorker.Utils;

public record Result<T>(IEnumerable<string> Errors, T? ParsedValue)
{
    public static Result<T> Invalid(IEnumerable<string> errors) => new(errors, default);
    public static Result<T> Valid(T value) => new([], value);
}
