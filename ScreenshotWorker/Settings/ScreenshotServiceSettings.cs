using System.ComponentModel.DataAnnotations;

namespace ScreenshotWorker.Settings;

public class ScreenshotServiceSettings
{
    [Required]
    public required string Url { get; set; }
}

