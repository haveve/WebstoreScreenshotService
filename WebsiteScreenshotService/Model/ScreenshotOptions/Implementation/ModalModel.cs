using System.ComponentModel.DataAnnotations;
using WebsiteScreenshotService.Utils.Attributes;

namespace WebsiteScreenshotService.Model.ScreenshotOptions.Implementation;

public class ModalModel: IModalModel
{
    [Required]
    public bool DismissDialogs { get; set; }

    [Required]
    public bool HidePopups { get; set; }

    /// <summary>
    /// CSS selectors to hide before screenshot.
    /// </summary>
    [SafeCssSelectorList(maxCount: 15, selectorMaxLength: 200)]
    public List<string> HideSelectors { get; set; } = [];
}
