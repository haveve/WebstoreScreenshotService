using System.ComponentModel.DataAnnotations;
using ScreenshotWorker.Utils.Attributes;

namespace ScreenshotWorker.Model.ScreenshotOptions;

public class ModalModel
{
    [Required]
    public bool DismissDialogs { get; set; }

    [Required]
    public bool HidePopups { get; set; }

    /// <summary>
    /// CSS selectors to hide before screenshot.
    /// </summary>
    [SafeCssSelectorList(30)]
    public List<string> HideSelectors { get; set; } = [];
}
