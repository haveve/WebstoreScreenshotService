using System.ComponentModel.DataAnnotations;

namespace WebsiteScreenshotService.Settings;

public class BrowserServiceSettings
{
    [Required]
    public double ScrollTimeout { get; set; }

    [Required]
    public double WaitForRequestsToCompleteTimeout { get; set; }

    [Required]
    public double PageLoadTimeout { get; set; }

    [Required]
    public double ScriptLoadTimeout { get; set; }

    [Required]
    public double DefaultDelay { get; set; }

    [Required]
    public double InitialPageLoadDelay { get; set; }
}
