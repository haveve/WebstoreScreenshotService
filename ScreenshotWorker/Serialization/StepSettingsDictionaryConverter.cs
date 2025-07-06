using System.Text.Json.Serialization;
using System.Text.Json;
using ScreenshotWorker.Services.ContentInitialization;

namespace ScreenshotWorker.Serialization;

public class StepSettingsDictionaryConverter : JsonConverter<Dictionary<string, ContentInitializationStepSettings>>
{
    public override Dictionary<string, ContentInitializationStepSettings>? Read(ref Utf8JsonReader reader, Type typeToConvert, JsonSerializerOptions options)
    {
        var result = new Dictionary<string, ContentInitializationStepSettings>();

        using var doc = JsonDocument.ParseValue(ref reader);
        foreach (var kvp in doc.RootElement.EnumerateObject())
        {
            string stepKey = kvp.Name;
            var json = kvp.Value.GetRawText();

            ContentInitializationStepSettings? step = stepKey switch
            {
                ContentInitializationStepsNames.RequestsToComplete => JsonSerializer.Deserialize<ContentInitializationStepSettings>(json, options),
                _ => JsonSerializer.Deserialize<ContentInitializationStepSettings>(json, options)
            };

            result[stepKey] = step!;
        }

        return result;
    }

    public override void Write(Utf8JsonWriter writer, Dictionary<string, ContentInitializationStepSettings> value, JsonSerializerOptions options)
    {
        writer.WriteStartObject();

        foreach (var kvp in value)
        {
            writer.WritePropertyName(kvp.Key);
            JsonSerializer.Serialize(writer, kvp.Value, kvp.Value.GetType(), options);
        }

        writer.WriteEndObject();
    }
}

