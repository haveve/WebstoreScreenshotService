using System.ComponentModel.DataAnnotations;

namespace WebsiteScreenshotService.Configurations;

public class ServerConfigurations
{
    [Required]
    public required int MaxRequestBodySize { get; set; }
}

