using System.ComponentModel.DataAnnotations;

namespace ScreenshotWorker.Settings.InitializationStep;

public class ScrollInitializationStepSettings : ContentInitializationStepSettings
{
    [Required]
    [Range(0, 360)]
    public double WaitForPossibleContentLoad { get; set; }

    [Required]
    [Range(0, 360)]
    public float PollingInterval { get; set; }
}