using System.ComponentModel.DataAnnotations;

namespace WebsiteScreenshotService.Model.UserInfo.Implementation;

public class UserInformation: IUserInformation
{
    [Required]
    public required Guid UserId { get; set; }
}
