using System.ComponentModel.DataAnnotations;
using Shared.Core.Contracts.ScreeshotModel.Components;

namespace Shared.Core.Contracts.ScreeshotModel;

public class MakeScreenshotModel
{
    [Required]
    public required string ConfirmationToken { get; set; }

    [Required]
    public required string ScreenshotId { get; set; }

    [Required]
    public required ScreenshotOptionsModel ScreenshotOptionsModel { get; set; }

    [Required]
    public required UserInformation UserInformation { get; set; }
}
