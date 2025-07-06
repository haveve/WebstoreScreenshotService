using System.ComponentModel.DataAnnotations;

namespace WebsiteScreenshotService.Model;

public class UserInformation
{
    [Required]
    public required Guid UserId { get; set; }
}
