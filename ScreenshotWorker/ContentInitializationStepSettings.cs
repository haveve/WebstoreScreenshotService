using System.ComponentModel.DataAnnotations;

namespace ScreenshotWorker;

public class ContentInitializationStepSettings
{
    [Required]
    [Range(0, 720)]
    public double ExecutionTimeout { get; set; }

    [Required]
    [Range(0, 360)]
    public double PoolingTimeout { get; set; }
}