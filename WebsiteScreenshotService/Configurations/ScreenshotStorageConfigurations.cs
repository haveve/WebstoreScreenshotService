using System.ComponentModel.DataAnnotations;

namespace ScreenshotWorker.Settings;

public class ScreenshotStorageConfigurations
{
    [Required]
    public required string Url { get; set; }
}
