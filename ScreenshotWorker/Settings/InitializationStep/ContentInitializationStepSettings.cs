using ScreenshotWorker.Services.ContentInitialization;
using System.ComponentModel.DataAnnotations;
using System.Text.Json.Serialization;

namespace ScreenshotWorker.Settings.InitializationStep;

[JsonDerivedType(typeof(ScrollInitializationStepSettings), typeDiscriminator: ContentInitializationStepsNames.Scroll)]
public class ContentInitializationStepSettings
{
    [Required]
    [Range(0, 720)]
    public float ExecutionTimeoutInSeconds { get; set; }
}