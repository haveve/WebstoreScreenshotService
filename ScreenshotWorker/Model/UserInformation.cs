using System.ComponentModel.DataAnnotations;

namespace ScreenshotWorker.Model;

public class UserInformation
{
    [Required]
    public required Guid UserId { get; set; }
}
