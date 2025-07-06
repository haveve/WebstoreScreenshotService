using System.ComponentModel.DataAnnotations;

namespace ScreenshotWorker;

public class ScrollInitializationStepSettings: ContentInitializationStepSettings
{
    [Required]
    [Range(0, 360)]
    public double WaitForPossibleContentLoad { get; set; }
}