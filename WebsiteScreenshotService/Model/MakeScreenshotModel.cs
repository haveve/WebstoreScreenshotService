using WebsiteScreenshotService.Model.UserInfo;
using WebsiteScreenshotService.Model.ScreenshotOptions;

namespace WebsiteScreenshotService.Model;

public class MakeScreenshotModel
{
    public required string ConfirmationToken { get; set; }

    public required string ScreenshotId { get; set; }

    public required IScreenshotOptionsModel ScreenshotOptionsModel { get; set; }

    public required IUserInformation UserInformation { get; set; }
}

