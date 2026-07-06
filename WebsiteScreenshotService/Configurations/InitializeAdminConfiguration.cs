using System.ComponentModel.DataAnnotations;

namespace WebsiteScreenshotService.Configurations;

public class InitializeAdminConfiguration
{
    [Required]
    public string Email { get; set; } = null!;
}
