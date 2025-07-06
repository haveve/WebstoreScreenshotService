using System.ComponentModel.DataAnnotations;
using System.Text.Json;

namespace ScreenshotWorker.Serialization;

public record Result<T>(IEnumerable<string> Errors, bool IsValid, T? ParsedValue)
{
    public static Result<T> Invalid(IEnumerable<string> errors) => new(errors, false, default);
    public static Result<T> Valid(T value) => new([], true, value);
}

public static class CustomJsonSerializer
{
    private readonly static JsonSerializerOptions _options = new()
    {
        PropertyNamingPolicy = JsonNamingPolicy.CamelCase,
    };

    public static string Serialize<T>(T obj)
       => JsonSerializer.Serialize(obj, _options);

    public static Result<T?> TryDeserialize<T>(ReadOnlySpan<byte> utf8Json)
    {
        var instance = JsonSerializer.Deserialize<T>(utf8Json);
        return ValidateInstance(instance);
    }

    public static Result<T?> TryDeserialize<T>(string json)
    {
        var instance = JsonSerializer.Deserialize<T>(json);
        return ValidateInstance(instance);
    }

    private static Result<T?> ValidateInstance<T>(T? instance)
    {
        if (instance is null)
            return Result<T?>.Invalid(["Instance cannot be empty"]);

        if (!IsModelValid(instance, out List<ValidationResult> validationResults))
        {
            var errors = validationResults
                .Where(r => r.ErrorMessage is not null)
                .Select(r => r.ErrorMessage!);

            return Result<T?>.Invalid(errors);
        }

        return Result<T?>.Valid(instance);
    }

    private static bool IsModelValid(object model, out List<ValidationResult> validationResults)
    {
        validationResults = [];

        var validationContext = new ValidationContext(model);
        return !Validator.TryValidateObject(model, validationContext, validationResults, validateAllProperties: true);
    }
}

