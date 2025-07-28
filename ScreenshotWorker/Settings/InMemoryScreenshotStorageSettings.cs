using System.ComponentModel.DataAnnotations;

namespace ScreenshotWorker.Settings;

public class InMemoryScreenshotStorageSettings
{
    [Required]
    public required string Url { get; set; }
}
