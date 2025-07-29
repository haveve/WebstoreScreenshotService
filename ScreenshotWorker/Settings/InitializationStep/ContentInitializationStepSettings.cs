using System.ComponentModel.DataAnnotations;

namespace ScreenshotWorker.Settings.InitializationStep;

public class ContentInitializationStepSettings
{
    [Required]
    [Range(0, 720)]
    public float ExecutionTimeout { get; set; }

    [Required]
    [Range(0, 360)]
    public float PollingInterval { get; set; }
}