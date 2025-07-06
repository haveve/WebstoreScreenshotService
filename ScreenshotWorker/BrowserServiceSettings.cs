using ScreenshotWorker.Services.ContentInitialization;
using System.ComponentModel.DataAnnotations;

namespace ScreenshotWorker;

public class BrowserServiceSettings : IValidatableObject
{
    [Required]
    public Dictionary<string, ContentInitializationStepSettings> ContentInitializationSteps { get; set; } = new();

    [Required]
    [Range(0, 720)]
    public double PageLoadTimeout { get; set; }

    [Required]
    [Range(0, 720)]
    public double ScriptLoadTimeout { get; set; }

    [Required]
    [Range(0, 720)]
    public double DefaultWaitTimeout { get; set; }

    [Required]
    [Range(0, 720)]
    public double InitialPageLoadTimeout { get; set; }

    public IEnumerable<ValidationResult> Validate(ValidationContext _)
       => ValidateContentInitializationStepsSettings();

    private IEnumerable<ValidationResult> ValidateContentInitializationStepsSettings()
    {
        foreach (var stepName in ContentInitializationStepsNames.Steps)
        {
            if (!ContentInitializationSteps.ContainsKey(stepName))
            {
                yield return new ValidationResult(
                    $"Missing required content initialization step: '{stepName}'.",
                    [nameof(ContentInitializationSteps)]);
            }
        }

        foreach (var kvp in ContentInitializationSteps)
        {
            var step = kvp.Value;
            var results = new List<ValidationResult>();
            var context = new ValidationContext(step);

            if (!Validator.TryValidateObject(step, context, results, validateAllProperties: true))
            {
                foreach (var validationResult in results)
                {
                    yield return new ValidationResult(
                        $"Step '{kvp.Key}' error: {validationResult.ErrorMessage}",
                        [$"{nameof(ContentInitializationSteps)}[{kvp.Key}]"]);
                }
            }
        }
    }
}
