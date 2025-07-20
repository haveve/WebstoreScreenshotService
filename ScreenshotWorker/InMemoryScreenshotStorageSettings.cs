using System.ComponentModel.DataAnnotations;

namespace ScreenshotWorker;

public class InMemoryScreenshotStorageSettings
{
    [Required]
    public required string Url { get; set; }
}
