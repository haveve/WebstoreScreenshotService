using System.ComponentModel.DataAnnotations;

namespace WebsiteScreenshotService.Settings;

public class ScreenshotStorageConfigurations
{
    [Required]
    public required string Url { get; set; }
}
