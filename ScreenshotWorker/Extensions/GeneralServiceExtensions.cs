using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Options;
using System.ComponentModel.DataAnnotations;
using System.Globalization;
using System.Text.Json;
using System.Text.Json.Serialization;

namespace ScreenshotWorker.Extensions;

public static class GeneralServiceExtensions
{
    public static string ToJson(this IConfigurationSection section)
    {
        object? BuildValue(IConfigurationSection sec)
        {
            var children = sec.GetChildren().ToList();
            if (children.Count == 0)
            {
                // Leaf node: try to parse as int, float, or bool, otherwise string
                var value = sec.Value;

                if (string.IsNullOrEmpty(value))
                    return null;

                // Try parse bool
                if (bool.TryParse(value, out var boolResult))
                    return boolResult;

                // Try parse int
                if (int.TryParse(value, NumberStyles.Integer, CultureInfo.InvariantCulture, out var intResult))
                    return intResult;

                // Try parse float/double
                if (double.TryParse(value, NumberStyles.Float, CultureInfo.InvariantCulture, out var doubleResult))
                    return doubleResult;

                // fallback string
                return value;
            }

            var dict = new Dictionary<string, object?>();
            foreach (var child in children)
            {
                dict[child.Key] = BuildValue(child);
            }
            return dict;
        }

        var obj = BuildValue(section);
        return JsonSerializer.Serialize(obj, new JsonSerializerOptions { WriteIndented = true });
    }

    public static IServiceCollection AddOptionsWithValidation<TOptions>(
        this IServiceCollection services,
        IConfigurationSection configSection,
        JsonSerializerOptions? jsonOptions = null
    ) where TOptions : class
    {
        // Manual deserialization (with full polymorphic support)
        var json = configSection.ToJson(); // workaround for .NET config binder limitations

        Console.WriteLine(json);

        var options = jsonOptions ?? new JsonSerializerOptions
        {
            PropertyNameCaseInsensitive = true,
        };
        options.Converters.Add(new AlwaysStringJsonConverter());

        var instance = JsonSerializer.Deserialize<TOptions>(json, options)
                       ?? throw new InvalidOperationException($"Failed to deserialize {typeof(TOptions).Name}");

        // Manual validation
        var validationContext = new ValidationContext(instance);
        var results = new List<ValidationResult>();
        if (!Validator.TryValidateObject(instance, validationContext, results, true))
        {
            var errors = string.Join("; ", results.Select(r => r.ErrorMessage));
            throw new ValidationException($"Validation failed for {typeof(TOptions).Name}: {errors}");
        }

        // Register as IOptions<T>
        services.AddSingleton<IOptions<TOptions>>(Options.Create(instance));
        return services;
    }

    public class AlwaysStringJsonConverter : JsonConverter<string>
    {
        public override string? Read(ref Utf8JsonReader reader, Type typeToConvert, JsonSerializerOptions options)
        {
            // Regardless of the token type, get its string representation
            switch (reader.TokenType)
            {
                case JsonTokenType.String:
                    return reader.GetString();

                case JsonTokenType.Number:
                    return reader.GetDouble().ToString(System.Globalization.CultureInfo.InvariantCulture);

                case JsonTokenType.True:
                    return "true";

                case JsonTokenType.False:
                    return "false";

                case JsonTokenType.Null:
                    return null;

                default:
                    throw new JsonException($"Unexpected token parsing string. TokenType: {reader.TokenType}");
            }
        }

        public override void Write(Utf8JsonWriter writer, string? value, JsonSerializerOptions options)
        {
            if (value is null)
                writer.WriteNullValue();
            else
                writer.WriteStringValue(value);
        }
    }
}
