using System.ComponentModel.DataAnnotations;

namespace WebsiteScreenshotService.Model;

public class MakeScreenshotModel
{
    [Required]
    public required string ScreenshotId { get; set; }

    [Required]
    public required ScreenshotOptionsModel ScreenshotOptionsModel { get; set; }

    [Required]
    public required UserInformation UserInformation { get; set; }
}

