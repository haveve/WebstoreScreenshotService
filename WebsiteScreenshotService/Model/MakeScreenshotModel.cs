using System.ComponentModel.DataAnnotations;

namespace WebsiteScreenshotService.Model;

public class MakeScreenshotModel
{
    public required string ConfirmationToken { get; set; }

    public required string ScreenshotId { get; set; }

    public required ScreenshotOptionsModel ScreenshotOptionsModel { get; set; }

    public required UserInformation UserInformation { get; set; }
}

