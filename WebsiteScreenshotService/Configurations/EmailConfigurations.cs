using System.ComponentModel.DataAnnotations;

namespace WebsiteScreenshotService.Configurations;

public class EmailConfigurations
{
    [Required]
    public string SenderAddress { get; set; } = default!;

    [Required]
    public string ConnectionString { get; set; } = default!;
}
