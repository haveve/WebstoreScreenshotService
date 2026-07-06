using System.ComponentModel.DataAnnotations;

namespace ScreenshotWorker.Settings;

public class LocalScreenshotStorageSettings
{
    [Required]
    public required string Url { get; set; }
}
