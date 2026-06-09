using System.ComponentModel.DataAnnotations;

namespace WebsiteScreenshotService.Configurations;

public class FrontendConfigurations
{
    [Required]
    public string BaseUrl { get; set; } = default!;
}
